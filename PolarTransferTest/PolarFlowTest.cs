using System;
using System.Configuration;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    [TestFixture]
    public class PolarFlowTest
    {
        private readonly PolarFlowManager polar = new PolarFlowManager();

        [Test]
        public void PolarTestLogin()
        {
            Assert.IsTrue(polar.LoginAsync(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]).GetAwaiter().GetResult());
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
            var training = new TrainingData()
            {
                ActivityType = Excercise.Running,
                Start = new DateTime(2016, 11, 5),
                Duration = new TimeSpan(1, 20, 25),
                Distance = 15.2,
                AvgHr = 140,
                Description = "test training",
            };
            polar.AddTrainingResultAsync(training).GetAwaiter().GetResult();
            var trainingData = polar.GetTrainingListAsync(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).GetAwaiter().GetResult().ToArray();
            Assert.AreEqual(1, trainingData.Length);
            polar.RemoveTrainingAsync(trainingData.Single().Id).GetAwaiter().GetResult();
            trainingData = polar.GetTrainingListAsync(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).GetAwaiter().GetResult().ToArray();
            Assert.IsFalse(trainingData.Any());
        }

        [Test]
        public void PolarTestGetTrainings()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingListAsync(new DateTime(2016, 11, 01)).GetAwaiter().GetResult();
            var trainingDatas = trainingData as TrainingData[] ?? trainingData.ToArray();

            Assert.AreEqual(15, trainingDatas.Length);
            Assert.AreEqual(0, trainingDatas.Count(data => data == null));
        }

        [Test]
        public void PolarTestChangeTraining()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingListAsync(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            Assert.AreEqual(1, trainingData.Length);
            var tr = polar.LoadTrainingDetailsAsync(trainingData[0]).GetAwaiter().GetResult();

            var oldDistance = tr.Distance;
            var oldDescription = tr.Description;

            tr.Distance = 9.8;
            tr.Description = "easy run";
            polar.UpdateTrainingDataAsync(tr).GetAwaiter().GetResult();
            trainingData = polar.GetTrainingListAsync(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            tr = polar.LoadTrainingDetailsAsync(trainingData[0]).GetAwaiter().GetResult();
            Assert.AreEqual("easy run", tr.Description);
            Assert.AreEqual(9.8, tr.Distance);

            tr.Distance = oldDistance;
            tr.Description = oldDescription;
            polar.UpdateTrainingDataAsync(tr).GetAwaiter().GetResult();
            trainingData = polar.GetTrainingListAsync(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).GetAwaiter().GetResult().ToArray();
            tr = polar.LoadTrainingDetailsAsync(trainingData[0]).GetAwaiter().GetResult();
            Assert.AreEqual(oldDescription, tr.Description);
            Assert.AreEqual(oldDistance, tr.Distance);
        }
    }
}
