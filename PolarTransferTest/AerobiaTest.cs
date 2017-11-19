﻿using System;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    /// <summary>
    /// Summary description for AerobiaTest
    /// </summary>
    [TestFixture]
    public class AerobiaTest
    {
        private readonly AerobiaManager aerobia;

        public AerobiaTest()
        {
            aerobia = new AerobiaManager();
        }

        [Test]
        public void AerobiaTestLogin()
        {
            Assert.IsTrue(aerobia.LoginAsync("aashmarin@gmail.com", "T@shk3nter").GetAwaiter().GetResult());
        }

        [Test]
        public void AerobiaTestExportTcx()
        {
            AerobiaTestLogin();
            try
            {
                var training = aerobia.GetTrainingFileTcxAsync("1369687").GetAwaiter().GetResult();
                Assert.DoesNotThrow(() => new XmlDocument().LoadXml(training));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void AerobiaTestGetTrainings()
        {
            AerobiaTestLogin();
            var trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 11, 01)).GetAwaiter().GetResult().ToArray();
            Assert.AreEqual(20, trainingData.Length);
            Assert.AreEqual(0, trainingData.Count(data => data == null));
        }

        [Test]
        public void AerobiaTestAddRemoveTrainingResult()
        {
            AerobiaTestLogin();
            var training = new TrainingData()
            {
                ActivityType = Excercise.Running,
                Start = new DateTime(2016, 11, 5, 8, 30, 0),
                Duration = new TimeSpan(1, 20, 25),
                Distance = 15.2,
                AvgHr = 140,
                Description = "test training",
            };
            aerobia.AddTrainingResultAsync(training).GetAwaiter().GetResult();
            var trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 11, 5)).GetAwaiter().GetResult();
            Assert.AreEqual(21, trainingData.Count());
            var added = aerobia.LoadTrainingDetailsAsync(trainingData.Single(tr => tr.Start.Day == 5)).GetAwaiter().GetResult();
            
            aerobia.RemoveTrainingAsync(trainingData.Single(tr => tr.Start.Day == 5).Id).GetAwaiter().GetResult();
            trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 11, 5)).GetAwaiter().GetResult();
            Assert.AreEqual(20, trainingData.Count());

            Assert.AreEqual("test training", added.Description);
        }

        [Test]
        public void AerobiaTestChangeTraining()
        {
            AerobiaTestLogin();
            var trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 12, 13)).GetAwaiter().GetResult().Single(tr => tr.Start.Day == 13);
            trainingData = aerobia.LoadTrainingDetailsAsync(trainingData).GetAwaiter().GetResult();
            var oldDistance = trainingData.Distance;
            var oldDescription = trainingData.Description;

            trainingData.Distance = 9.8;
            trainingData.Description = "easy run";
            try
            {
                aerobia.UpdateTrainingDataAsync(trainingData).GetAwaiter().GetResult();
            }
            catch
            {
                // TODO : somehow error 500 occures, but result is ok
            }
            trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 12, 13)).GetAwaiter().GetResult().Single(tr => tr.Start.Day == 13);
            trainingData = aerobia.LoadTrainingDetailsAsync(trainingData).GetAwaiter().GetResult();
            Assert.AreEqual("easy run", trainingData.Description);
            Assert.AreEqual(9.8, trainingData.Distance);

            trainingData.Distance = oldDistance;
            trainingData.Description = oldDescription;
            try
            {
                aerobia.UpdateTrainingDataAsync(trainingData).GetAwaiter().GetResult();
            }
            catch
            {
                // TODO : somehow error 500 occures, but result is ok
            }
            trainingData = aerobia.GetTrainingListAsync(new DateTime(2016, 12, 13)).GetAwaiter().GetResult().Single(tr => tr.Start.Day == 13);
            trainingData = aerobia.LoadTrainingDetailsAsync(trainingData).GetAwaiter().GetResult();
            Assert.AreEqual(oldDescription, trainingData.Description);
            Assert.AreEqual(oldDistance, trainingData.Distance);
        }

        [Test]
        public void AerobiaTestUploadFile()
        {
            AerobiaTestLogin();
            aerobia.UploadTcxAsync(TestHeler.Sampletcx).GetAwaiter().GetResult();
            var recentlyAdded = aerobia.GetTrainingListAsync(new DateTime(2016, 11, 6)).GetAwaiter().GetResult().Where(tr => tr.Start.Day == 6).ToArray();
            Assert.AreEqual(1, recentlyAdded.Length);
            aerobia.RemoveTrainingAsync(recentlyAdded[0].Id).GetAwaiter().GetResult();
        }
    }
}
