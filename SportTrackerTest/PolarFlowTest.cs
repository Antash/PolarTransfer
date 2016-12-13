using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportTrackerManager.Core;
using System.Linq;

namespace SportTrackerTest
{
    /// <summary>
    /// Summary description for PolarFlowTest
    /// </summary>
    [TestClass]
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

        [TestMethod]
        public void PolarTestLogin()
        {
            Assert.IsTrue(polar.Login("aashmarin@gmail.com", "1qaz2wsx"));
        }

        [TestMethod]
        public void PolarTestExportTcx()
        {
            PolarTestLogin();
            try
            {
                var training = polar.GetTrainingFileTcx("491398793");
                Assert.AreEqual(1150391, training.Length);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
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
                var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6));
                Assert.AreEqual(1, trainingData.Count());
                try
                {
                    // somehow returns 400 but works well
                    polar.RemoveTraining(trainingData.Single().Id);
                }
                catch { }
                trainingData = polar.GetTrainingList(new DateTime(2016, 11, 5), new DateTime(2016, 11, 6));
                Assert.IsFalse(trainingData.Any());
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void PolarTestGetTrainings()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingList(new DateTime(2016, 11, 01));
            Assert.AreEqual(15, trainingData.Count());
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }

        [TestMethod]
        public void PolarTestChangeTraining()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingList(new DateTime(2016, 12, 11), new DateTime(2016, 12, 11)).ToArray();
            Assert.AreEqual(1, trainingData.Count());
            trainingData[0].Description = "long run. Very cold.";
            polar.UpdateTrainingData(trainingData[0]);
            trainingData = polar.GetTrainingList(new DateTime(2016, 12, 11), new DateTime(2016, 12, 11)).ToArray();
            Assert.AreEqual("long run. Very cold.", trainingData[0].Description);
        }
    }
}
