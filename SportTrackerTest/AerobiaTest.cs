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
            Assert.IsTrue(aerobia.Login("aashmarin%40gmail.com", "T%40shk3nter"));
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
    }
}
