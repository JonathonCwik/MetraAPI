using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetraAPI;

namespace MetraAPI.Tests
{
    [TestFixture]
    public class ApiIntegrationTests
    {
        [Test]
        public void GetLines_AllLines_HasLinesNoStations()
        {
            var lines = MetraAPI.GetLines();

            Assert.IsNotEmpty(lines);
            Assert.That(lines.Count(l => l.Stations.Count > 0) == 0);
        }

        [Test]
        public void GetLinesAndStations_AllLinesAndStations_HasLinesAndStations()
        {
            var lines = MetraAPI.GetLinesAndStations();

            Assert.IsNotEmpty(lines);
            foreach (var line in lines)
            {
                Assert.IsNotEmpty(line.Stations);
            }
        }


    }
}
