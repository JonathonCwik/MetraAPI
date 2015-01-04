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
        public void GetStationForLine_UPW_ReturnsUPWStops()
        {
            var stations = MetraAPI.GetStationsForLine("UP-W");

            Assert.That(stations.Count() > 0);
        }

        [Test]
        public void UpdateAllStationsForAllLines_UpdatesAllStations()
        {
            LineHandler.UpdateAllStationsForAllLines();

            var lines = LineHandler.GetAllLines();

            foreach (var line in lines)
            {
                Assert.That(line.Stations.Count > 0);
            }
        }

        [Test]
        public void GetNextTrainBatch_UPW_VillaPark_OTC_Success()
        {
            var trainBatch = MetraAPI.GetNextTrainBatch("UP-W", "VILLAPARK", "OTC");

            Assert.IsNotEmpty(trainBatch);
        }

        [Test]
        public void GetLineAndStationIds_All_ReturnsLineAndStationIds()
        {
            var linesAndStations = MetraAPI.GetLineAndStationIds();

            Assert.IsNotEmpty(linesAndStations);
        }
    }
}
