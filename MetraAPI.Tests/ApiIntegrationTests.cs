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

        [Test]
        public void GetAllTrainDelays_NoParams_HasDelays()
        {
            var delays = MetraAPI.GetAllTrainDelays();

            Assert.IsNotEmpty(delays);
        }

        [Test]
        public void GetTrainData_UPWVillaParkToOgilvyObjects_ReturnsTrainData()
        {
            var lines = MetraAPI.GetLinesAndStations();

            var UPWLine = lines.Single(l => l.LookupName == "up-w");
            var vpStation = UPWLine.Stations.Single(s => s.Station == "VILLAPARK");
            var ogilvyStation = UPWLine.Stations.Single(s => s.Station == "OTC");

            Assert.IsNotEmpty(MetraAPI.GetTrainData(UPWLine, vpStation, ogilvyStation));
        }

        [Test]
        public void GetTrainData_UPWVillaParkToOgilvyStrings_ReturnsTrainData()
        {
            Assert.IsNotEmpty(MetraAPI.GetTrainData("up-w", "VILLAPARK", "OTC"));
        }

        [Test]
        public void GetTrainNumbersForLine_UPW_ReturnsTrainNumbers()
        {
            Assert.IsNotEmpty(MetraAPI.GetTrainNumbersForLine("UP West"));
        }

    }
}
