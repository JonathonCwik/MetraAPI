﻿using System;
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
        private const string GET_STATIONS_FROM_LINE_URL = "http://metrarail.com/content/metra/wap/en/home/RailTimeTracker/jcr:content/trainTracker.get_stations_from_line.json?";
        private const string GET_TRAIN_DATA_URL = "http://metrarail.com/content/metra/en/home/jcr:content/trainTracker.get_train_data.json?";
        private const string GET_LINE_AND_STATION_ID_URL = "http://metrarail.com/content/metra/en/home/jcr:content/trainTracker.lataexport.html";

        private enum Methods { get_stations_from_line, get_train_data };
        private enum ReturnType { json };

        /// <summary>
        /// Retrieves List of Lines and Stations with Ids populated
        /// </summary>
        /// <returns></returns>
        public static List<Line> GetLineAndStationIds()
        {
            var lines = new List<Line>();

            var request = WebRequest.Create(GET_LINE_AND_STATION_ID_URL);
            var response = GetResponseData(request);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            doc.DocumentNode.SelectSingleNode("script").Remove();
            var content = doc.DocumentNode.InnerHtml.Trim();
            string[] stringSeparators = new string[] { "<br>" };
            var lineStationArray = content.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var lS in lineStationArray)
            {
                var split = lS.Split(',');
                if (lines.Count(l => l.Id == split[0]) == 0)
                {
                    lines.Add(
                        new Line
                        {
                            Id = split[0],
                            Stations = new List<Station> 
                            { 
                                new Station { Id = split[1] }
                            }
                        }
                    );
                }
                else
                {
                    lines.Single(l => l.Id == split[0])
                        .Stations.Add(
                        new Station { Id = split[1] }
                    );
                }
            }

            return lines.OrderBy(l => l.Id).ToList();
        }

        /// <summary>
        /// Retrieve stations for specific line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>List of Stations for Line</returns>
        public static List<Station> GetStationsForLine(Line line)
        {
            return GetStationsForLine(line.Id);
        }
        
        /// <summary>
        /// Retrieve stations for specific line
        /// </summary>
        /// <param name="lineId">Line Id (e.g. UP-NW, ME)</param>
        /// <returns></returns>
        public static List<Station> GetStationsForLine(string lineId)
        {
            var url = GET_STATIONS_FROM_LINE_URL;

            url = AddParamToUrl(url, "trackerNumber", "0");
            url = AddParamToUrl(url, "trainLineId", lineId);

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

        /// <summary>
        /// Gets Next 5 Trains for line, origin station, and destination station
        /// </summary>
        /// <param name="line">Target Line</param>
        /// <param name="originStation">Origin Station</param>
        /// <param name="destinationStation">Destination Station</param>
        /// <returns>Batch of Trains</returns>
        public static List<Train> GetNextTrainBatch(Line line, Station originStation, Station destinationStation)
        {
            return GetNextTrainBatch(line.Id, originStation.Id, destinationStation.Id);
        }

        /// <summary>
        /// Gets Next 5 Trains for line, origin station, and destination station
        /// </summary>
        /// <param name="lineId">Target Line's Id (e.g. UP-W, UP-NW, etc)</param>
        /// <param name="originStation">Origin Station</param>
        /// <param name="destinationStation">Destination Station</param>
        /// <returns>Batch of Trains</returns>
        public static List<Train> GetNextTrainBatch(string lineId, Station originStation, Station destinationStation)
        {
            return GetNextTrainBatch(lineId, originStation.Id, destinationStation.Id);
        }

        /// <summary>
        /// Gets Next 5 Trains for line, origin station, and destination station
        /// </summary>
        /// <param name="line">Target Line</param>
        /// <param name="originStationId">Origin Station Id</param>
        /// <param name="destinationStationId">Destination Station Id</param>
        /// <returns></returns>
        public static List<Train> GetNextTrainBatch(Line line, string originStationId, string destinationStationId)
        {
            return GetNextTrainBatch(line.Id, originStationId, destinationStationId);
        }

        /// <summary>
        /// Gets Next 5 Trains for line, origin station, and destination station
        /// </summary>
        /// <param name="lineId">Target Line Id</param>
        /// <param name="originStationId">Origin Station Id</param>
        /// <param name="destinationStationId">Destination Station Id</param>
        /// <returns></returns>
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
