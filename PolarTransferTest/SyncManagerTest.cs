using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    [TestFixture]
    public class SyncManagerTest
    {
        private readonly SyncManager.SyncManager sync = new SyncManager.SyncManager();

        [SetUp]
        public void SetUp()
        {
            var aerobia = new AerobiaManager();
            var flow = new PolarFlowManager();
            sync.AddSource(aerobia);
            sync.AddSource(flow);

            aerobia.LoginAsync(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"])
                .GetAwaiter().GetResult();

            flow.LoginAsync(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"])
                .GetAwaiter().GetResult();
        }

        [Test]
        public void SyncTest()
        {
            var diff = sync.Fetch(new DateTime(2017, 9, 21), new DateTime(2017, 9, 23)).GetAwaiter().GetResult();

            Assert.AreEqual(2, diff.TrainingDataDictionary.Keys.Count());
            Assert.AreEqual(4, diff.TrainingDataDictionary["aerobia"].Count());
            Assert.AreEqual(3, diff.TrainingDataDictionary["polar"].Count());
        }
    }
}