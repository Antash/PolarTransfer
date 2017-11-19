using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SportTrackerManager.Core;

namespace PolarTransferTest
{
    [TestFixture]
    public class StravaTest
    {
        private readonly StravaManager strava = new StravaManager();

        [Test]
        public void StravaTestLogin()
        {
            Assert.IsTrue(strava.LoginAsync("21552", "").GetAwaiter().GetResult());
        }
    }
}
