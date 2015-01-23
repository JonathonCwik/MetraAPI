using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetraAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace MetraAPI
{
    public static class MetraAPI
    {
        private const string serviceUrl = "http://12.205.200.243/AJAXTrainTracker.svc/";

        /// <summary>
        /// Retrieves List of Lines
        /// </summary>
        /// <returns></returns>
        public static List<Line> GetLines()
        {
            var lines = new List<Line>();

            var request = WebRequest.Create(serviceUrl + "GetCorridorLookup");
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            return JsonConvert.DeserializeObject<List<Line>>(resultObj["d"].Value);
        }

        /// <summary>
        /// Retrieves List of Lines and Stations
        /// </summary>
        /// <returns></returns>
        public static List<Line> GetLinesAndStations()
        {
            var lines = new List<Line>();

            var request = WebRequest.Create(serviceUrl + "GetCorridorLookup");
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            lines = JsonConvert.DeserializeObject<List<Line>>(resultObj["d"].Value);

            var allStations = GetAllStations();

            foreach (var station in allStations)
            {
                var stations = lines.Single(l => l.CorridorName == station.CorridorName).Stations;

                if (stations.Count(s => s.StationName == station.StationName) == 0)
                {
                    stations.Add(station);
                }
            }

            return lines;
        }

        /// <summary>
        /// Retrieves All Stations
        /// </summary>
        /// <returns></returns>
        public static List<TrainStation> GetAllStations()
        {
            var stations = new List<TrainStation>();

            var request = WebRequest.Create(serviceUrl + "GetCorridorStationLookup");
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            return JsonConvert.DeserializeObject<List<TrainStation>>(resultObj["d"].Value);
        }


        private static string AddParamToUrl(string url, string key, string value)
        {
            return url += key + "=" + value + "&";
        }

        private static string GetResponseData(WebRequest request)
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            var status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            var dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}
