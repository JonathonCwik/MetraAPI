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
using System.Runtime.Serialization.Formatters.Binary;

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

        /// <summary>
        /// Retrieves all trains delay info
        /// </summary>
        /// <returns></returns>
        public static List<TrainDelay> GetAllTrainDelays()
        {
            var delays = new List<TrainDelay>();

            var request = WebRequest.Create(serviceUrl + "GetTrainDelays");
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            delays = JsonConvert.DeserializeObject<List<TrainDelay>>(resultObj["d"].Value);

            // DateTimes are off by 6 hours
            delays.ForEach(d => d.LastUpdate = d.LastUpdate);
            delays.ForEach(d => d.Timestamp = d.Timestamp);

            return delays;
        }

        /// <summary>
        /// Get train data for next batch of trains from origin to destination
        /// </summary>
        /// <param name="line">Line (e.g. UP-W)</param>
        /// <param name="origin">Origin Station (e.g. VILLAPARK)</param>
        /// <param name="destination">Destination Station (e.g. OTC)</param>
        /// <returns></returns>
        public static List<TrainData> GetTrainData(string line, string origin, string destination)
        {
            if (line == null || destination == null || origin == null)
            {
                throw new ArgumentNullException("Line LookupName, Origin Station, and Destination Station are all required fields");
            }

            var trainData = new List<TrainData>();

            var request = WebRequest.Create(serviceUrl + "GetAcquityTrainData");
            request.Method = "POST";

            var trainDataRequest = new GetTrainDataRequest
            {
                stationRequest = new StationRequest
                {
                    Corridor = line,
                    Destination = destination,
                    Origin = origin
                }
            };

            request = ConvertToPostRequest(request,
                trainDataRequest,
                "application/json");

            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            resultObj = JsonConvert.DeserializeObject(resultObj["d"].Value);

            foreach (var tData in resultObj)
            {
                if (tData.Name.Contains("train"))
                {
                    trainData.Add(new TrainData
                    {
                        DepartingStation = tData.Value["dpt_station"],
                        TrainNumber = tData.Value["train_num"],
                        ScheduledDepartureTime = ((DateTime)tData.Value["scheduled_dpt_time"]),
                        EstimatedDepartureTime = ((DateTime)tData.Value["estimated_dpt_time"]),
                        IsModified = tData.Value["is_modified"],
                        Timestamp = ((DateTime)tData.Value["timestamp"]),
                        DateAge = tData.Value["DateAge"],
                        IsDuplicate = tData.Value["is_duplicate"],
                        RunState = tData.Value["RunState"]
                    });
                }
            }

            return trainData;
        }

        /// <summary>
        /// Get train data for next batch of trains from origin to destination
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="origin">Origin Station</param>
        /// <param name="destination">Destination Station</param>
        /// <returns></returns>
        public static List<TrainData> GetTrainData(Line line, TrainStation origin, TrainStation destination)
        {
            return GetTrainData(line.LookupName, origin.StationName, destination.StationName);
        }

        /// <summary>
        /// Retrieves train numbers for line
        /// </summary>
        /// <param name="line">Line Name (e.g. UP West)</param>
        /// <returns>Array of Train Numbers</returns>
        public static int[] GetTrainNumbersForLine(string line)
        {
            var url = AddParamToUrl(serviceUrl + "GetTrainsByCorridors?", "Corridor", line);

            var request = WebRequest.Create(url);
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            return JsonConvert.DeserializeObject<int[]>(resultObj["d"].Value);
        }

        /// <summary>
        /// Retrieves train numbers for line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>Array of Train Numbers</returns>
        public static int[] GetTrainNumbersForLine(Line line)
        {
            if (String.IsNullOrEmpty(line.CorridorName))
            {
                throw new ArgumentException("Line CorridorName cannot be null or empty. Either populate it by hand or retrieve the line via the api");
            }
            return GetTrainNumbersForLine(line.CorridorName);
        }

        /// <summary>
        /// Retrieves train schedule
        /// </summary>
        /// <param name="line">Line Name (e.g. UP West)</param>
        /// <param name="trainNum">Train Number</param>
        /// <returns>Train Schedule</returns>
        public static List<TrainSchedule> GetTrainSchedule(string line, int trainNum)
        {
            var url = serviceUrl + "GetSchedules?";
            url = AddParamToUrl(url, "TrainNumber", trainNum.ToString());
            url = AddParamToUrl(url, "Corridor", line);

            var request = WebRequest.Create(url);
            var response = GetResponseData(request);

            dynamic resultObj = JsonConvert.DeserializeObject(response);

            return JsonConvert.DeserializeObject<List<TrainSchedule>>(resultObj["d"].Value);
        }

        public static List<TrainSchedule> GetTrainSchedule(Line line, int trainNum)
        {
            if (String.IsNullOrEmpty(line.CorridorName))
            {
                throw new ArgumentException("Line CorridorName cannot be null or empty. Either populate it by hand or retrieve the line via the api");
            }

            return GetTrainSchedule(line.CorridorName, trainNum);
        }

        #region Helper Methods

        private static string AddParamToUrl(string url, string key, string value)
        {
            return url += key + "=" + value + "&";
        }

        private static WebRequest ConvertToPostRequest(WebRequest webRequest, object content, string contentType)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = contentType;
            var byteArray = ObjectToJsonByteArray(content);
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            return webRequest;
        }

        // Convert an object to a byte array
        private static byte[] ObjectToJsonByteArray(Object obj)
        {
            var json = JsonConvert.SerializeObject(obj).Trim();

            if (json == null)
                return null;

            byte[] bytes = new byte[json.Length * sizeof(char)];
            System.Buffer.BlockCopy(json.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
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

        #endregion
    }
}
