using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class Line
    {
        public string LookupName { get; set; }
        public string CorridorName { get; set; }
        public string Abbrevation { get; set; }
        public string CorridorGroup { get; set; }
        public bool Shutdown { get; set; }
        public List<TrainStation> Stations = new List<TrainStation>();
    }
}
