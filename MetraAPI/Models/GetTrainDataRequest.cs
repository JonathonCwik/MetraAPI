using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    [Serializable]
    public class GetTrainDataRequest
    {
        public StationRequest stationRequest { get; set; }
    }

    [Serializable]
    public class StationRequest
    {
        public string Corridor { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
    }
}
