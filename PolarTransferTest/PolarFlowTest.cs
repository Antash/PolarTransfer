using System;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    /// <summary>
    /// Summary description for PolarFlowTest
    /// </summary>
    [TestFixture]
    public class PolarFlowTest
    {
        private readonly PolarFlowManager polar;

        public PolarFlowTest()
        {
            polar = new PolarFlowManager();
        }

        [Test]
        public void PolarTestLogin()
        {
            Assert.IsTrue(polar.Login("aashmarin@gmail.com", "1qaz2wsx").GetAwaiter().GetResult());
        }

        [Test]
        public void PolarTestExportTcx()
        {
            PolarTestLogin();
            try
            {
                var training = polar.GetTrainingFileTcxAsync("491398793").GetAwaiter().GetResult();
                Assert.DoesNotThrow(() => new XmlDocument().LoadXml(training));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void PolarTestAddRemoveTrainingResult()
        {
            PolarTestLogin();
            try
            {
                var training = new TrainingData()
                {
                    ActivityType = Excercise.Running,
                    Start = new DateTime(2016, 11, 5),
                    Duration = new TimeSpan(1, 20, 25),
                    Distance = 15.2,
                    AvgHr = 140,
                    Description = "test training",
                };
                polar.AddTrainingResult(training).GetAwaiter().GetResult();
                var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).GetAwaiter().GetResult().ToArray();
                Assert.AreEqual(1, trainingData.Count());
                var added = polar.LoadTrainingDetails(trainingData[0]);
                try
                {
                    // somehow returns 400 but works well
                    polar.RemoveTraining(trainingData.Single().Id).GetAwaiter().GetResult(); ;
                }
                catch
                {
                    
                }
                trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).GetAwaiter().GetResult().ToArray();
                Assert.IsFalse(trainingData.Any());
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void PolarTestGetTrainings()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 01)).GetAwaiter().GetResult();
            Assert.AreEqual(15, trainingData.Count());
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }

        [Test]
        public void PolarTestChangeTraining()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            Assert.AreEqual(1, trainingData.Count());
            var tr = polar.LoadTrainingDetails(trainingData[0]).GetAwaiter().GetResult();

            var oldDistance = tr.Distance;
            var oldDescription = tr.Description;

            tr.Distance = 9.8;
            tr.Description = "easy run";
            polar.UpdateTrainingData(tr).GetAwaiter().GetResult();
            trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            tr = polar.LoadTrainingDetails(trainingData[0]).GetAwaiter().GetResult();
            Assert.AreEqual("easy run", tr.Description);
            Assert.AreEqual(9.8, tr.Distance);

            tr.Distance = oldDistance;
            tr.Description = oldDescription;
            polar.UpdateTrainingData(tr).GetAwaiter().GetResult();
            trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            tr = polar.LoadTrainingDetails(trainingData[0]).GetAwaiter().GetResult();
            Assert.AreEqual(oldDescription, tr.Description);
            Assert.AreEqual(oldDistance, tr.Distance);
        }
    }
}
