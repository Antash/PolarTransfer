using System;
using System.Text;
using System.Collections.Generic;
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
            Assert.IsTrue(polar.Login("aashmarin%40gmail.com", "1qaz2wsx"));
        }

        [TestMethod]
        public void PolarTestExportTcx()
        {
            PolarTestLogin();
            try
            {
                var training = polar.GetTrainingFileTcx(new TrainingData("491398793"));
                Assert.AreEqual(1150391, training.Length);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void PolarTestAddTrainingResult()
        {
            PolarTestLogin();
            //TODO fix
            Assert.IsTrue(polar.AddTrainingResult(new TrainingData("1")));
        }

        [TestMethod]
        public void PolarTestGetTrainings()
        {
            PolarTestLogin();
            var trainingData = polar.GetExercises(new DateTime(2016, 11, 01));
            Assert.AreEqual(15, trainingData.Count());
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }
    }
}
