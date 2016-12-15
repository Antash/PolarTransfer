using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportTrackerManager.Core;
using System.Linq;

namespace SportTrackerTest
{
    /// <summary>
    /// Summary description for AerobiaTest
    /// </summary>
    [TestClass]
    public class AerobiaTest
    {
        ISportTrackerManager aerobia;

        public AerobiaTest()
        {
            aerobia = new AerobiaManager();
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
        public void AerobiaTestLogin()
        {
            Assert.IsTrue(aerobia.Login("aashmarin@gmail.com", "T@shk3nter"));
        }

        [TestMethod]
        public void AerobiaTestExportTcx()
        {
            AerobiaTestLogin();
            try
            {
                var training = aerobia.GetTrainingFileTcx("1369687");
                Assert.AreEqual(1957114, training.Length);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void AerobiaTestGetTrainings()
        {
            AerobiaTestLogin();
            var trainingData = aerobia.GetTrainingList(new DateTime(2016, 11, 01)).ToArray();
            Assert.AreEqual(20, trainingData.Count());
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }

        [TestMethod]
        public void AerobiaTestAddRemoveTrainingResult()
        {
            AerobiaTestLogin();
            var training = new TrainingData()
            {
                ActivityType = Excercise.Running,
                Start = new DateTime(2016, 11, 5),
                Duration = new TimeSpan(1, 20, 25),
                Distance = 15.2,
                AvgHr = 140,
                Description = "test training",
            };
            aerobia.AddTrainingResult(training);
            var trainingData = aerobia.GetTrainingList(new DateTime(2016, 11, 5));
            Assert.AreEqual(21, trainingData.Count());
            var added = aerobia.LoadTrainingDetails(trainingData.Single(tr => tr.Start.Day == 5));
            Assert.AreEqual("test training", added.Description);
            aerobia.RemoveTraining(trainingData.Single(tr => tr.Start.Day == 5).Id);
            trainingData = aerobia.GetTrainingList(new DateTime(2016, 11, 5));
            Assert.AreEqual(20, trainingData.Count());
        }

        [TestMethod]
        public void AerobiaTestChangeTraining()
        {/*
            AerobiaTestLogin();
            var trainingData = aerobia.GetTrainingList(new DateTime(2016, 12, 13)).Where(tr => tr.Start.Day == 13).ToArray();
            Assert.AreEqual(1, trainingData.Count());
            var oldDistance = trainingData[0].Distance;
            var oldDescription = trainingData[0].Description;

            trainingData[0].Distance = 9.8;
            trainingData[0].Description = "easy run";
            aerobia.UpdateTrainingData(trainingData[0]);
            trainingData = aerobia.GetTrainingList(new DateTime(2016, 12, 13)).Where(tr => tr.Start.Day == 13).ToArray();
            Assert.AreEqual("easy run", trainingData[0].Description);
            Assert.AreEqual(9.8, trainingData[0].Distance);

            trainingData[0].Distance = oldDistance;
            trainingData[0].Description = oldDescription;
            aerobia.UpdateTrainingData(trainingData[0]);
            trainingData = aerobia.GetTrainingList(new DateTime(2016, 12, 13)).Where(tr => tr.Start.Day == 13).ToArray();
            Assert.AreEqual(oldDescription, trainingData[0].Description);
            Assert.AreEqual(oldDistance, trainingData[0].Distance);*/
        }
    }
}
