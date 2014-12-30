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

namespace MetraAPI
{
    public static class MetraAPI
    {
        private const string GET_STATIONS_FROM_LINE_URL = "http://metrarail.com/content/metra/wap/en/home/RailTimeTracker/jcr:content/trainTracker.get_stations_from_line.json?";
        private const string GET_TRAIN_DATA_URL = "http://metrarail.com/content/metra/en/home/jcr:content/trainTracker.get_train_data.json?";

        private enum Methods { get_stations_from_line, get_train_data };
        private enum ReturnType { json };

        public static List<Station> GetStationsForLine(Line line)
        {
            var url = GET_STATIONS_FROM_LINE_URL;
            
            url = AddParamToUrl(url, "trackerNumber", "0");
            url = AddParamToUrl(url, "trainLineId", line.Id);

            var request = WebRequest.Create(url);
            var response = GetResponseData(request);

            dynamic responseObj = JsonConvert.DeserializeObject(response);

            var stations = new List<Station>();

            foreach (var station in responseObj.stations)
            {
                stations.Add(new Station
                {
                    Id = station.Value["id"],
                    Name = station.Value["name"],
                    Order = int.Parse(station.Name)
                });
            }

            return stations;
        }

        public static List<Station> GetStationsForLine(string lineId)
        {
            return GetStationsForLine(
                new Line
                {
                    Id = lineId
                }
            );
        }

        public static List<Train> GetNextTrainBatch(Line line, Station originStation, Station destinationStation)
        {
            return GetNextTrainBatch(line.Id, originStation.Id, destinationStation.Id);
        }

        public static List<Train> GetNextTrainBatch(string lineId, Station originStation, Station destinationStation)
        {
            return GetNextTrainBatch(lineId, originStation.Id, destinationStation.Id);
        }

        public static List<Train> GetNextTrainBatch(Line line, string originStationId, string destinationStationId)
        {
            return GetNextTrainBatch(line.Id, originStationId, destinationStationId);
        }

        public static List<Train> GetNextTrainBatch(string lineId, string originStationId, string destinationStationId)
        {
            var url = GET_TRAIN_DATA_URL;

            url = AddParamToUrl(url, "line", lineId);
            url = AddParamToUrl(url, "origin", originStationId);
            url = AddParamToUrl(url, "destination", destinationStationId);

            var request = WebRequest.Create(url);
            var response = GetResponseData(request);

            dynamic responseObj = JsonConvert.DeserializeObject(response);

            var trainBatch = new List<Train>();

            foreach (var train in responseObj)
            {
                trainBatch.Add(new Train
                {
                    EstimatedArvTime = train.Value["estimated_arv_time"] ?? new DateTime(),
                    EstimatedDptTime = train.Value["estimated_dpt_time"] ?? new DateTime(),
                    HasData = train.Value["hasData"] ?? false,
                    HasDelay = train.Value["hasDelay"] ?? false,
                    IsRed = train.Value["isRed"] ?? false,
                    NotDeparted = train.Value["notDeparted"] ?? false,
                    ScheduledArvTime = train.Value["scheduled_arv_time"] ?? new DateTime(),
                    ScheduledDptTime = train.Value["scheduled_dpt_time"] ?? new DateTime(),
                    Status = train.Value["status"] ?? -1,
                    Timestamp = train.Value["timestamp"] ?? 0,
                    TrainNum = train.Value["train_num"] ?? 0,
                    TripId = train.Value["trip_id"] ?? 0
                });
            }

            return trainBatch;
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
