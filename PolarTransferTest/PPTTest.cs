using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    [TestFixture]
    public class PPTTest
    {
        private readonly PPTManager polar = new PPTManager();

        [Test]
        public void PolarTestLogin()
        {

            Assert.IsTrue(polar.LoginAsync(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]).GetAwaiter().GetResult());
        }

        [Test]
        public void PolarTestGetTrainings()
        {
            PolarTestLogin();
            var trainingData = polar.GetTrainingListAsync(new DateTime(2014, 11, 01), new DateTime(2014, 12, 01)).GetAwaiter().GetResult();
            var trainingDatas = trainingData as TrainingData[] ?? trainingData.ToArray();

            Assert.AreEqual(15, trainingDatas.Length);
            //Assert.AreEqual(0, trainingDatas.Count(data => data == null));
        }
    }
}