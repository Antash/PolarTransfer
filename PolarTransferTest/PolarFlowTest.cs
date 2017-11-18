using System;
using SportTrackerManager.Core;
using System.Linq;
using NUnit.Framework;

namespace SportTrackerTest
{
    /// <summary>
    /// Summary description for PolarFlowTest
    /// </summary>
    [TestFixture]
    public class PolarFlowTest
    {
        ISportTrackerManager polar;

        public PolarFlowTest()
        {
            polar = new PolarFlowManager();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

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
                var training = polar.GetTrainingFileTcx("491398793");
                Assert.AreEqual(1148339, training.Length);
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
                polar.AddTrainingResult(training);
                var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).ToArray();
                Assert.AreEqual(1, trainingData.Count());
                var added = polar.LoadTrainingDetails(trainingData[0]);
                try
                {
                    // somehow returns 400 but works well
                    polar.RemoveTraining(trainingData.Single().Id);
                }
                catch { }
                trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6)).ToArray();
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
            var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 01));
            Assert.AreEqual(15, trainingData.Count());
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }

        [Test]
        public void PolarTestChangeTraining()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).ToArray();
            Assert.AreEqual(1, trainingData.Count());
            var tr = polar.LoadTrainingDetails(trainingData[0]);

            var oldDistance = tr.Distance;
            var oldDescription = tr.Description;

            tr.Distance = 9.8;
            tr.Description = "easy run";
            polar.UpdateTrainingData(tr);
            trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).ToArray();
            tr = polar.LoadTrainingDetails(trainingData[0]);
            Assert.AreEqual("easy run", tr.Description);
            Assert.AreEqual(9.8, tr.Distance);

            tr.Distance = oldDistance;
            tr.Description = oldDescription;
            polar.UpdateTrainingData(tr);
            trainingData = polar.GetTrainingList(new DateTime(2016, 12, 13), new DateTime(2016, 12, 13)).ToArray();
            tr = polar.LoadTrainingDetails(trainingData[0]);
            Assert.AreEqual(oldDescription, tr.Description);
            Assert.AreEqual(oldDistance, tr.Distance);
        }
    }
}
