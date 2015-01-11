using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class TrainDelay
    {
        public int TrainNumber { get; set; }
        public int DelayTime { get; set; }
        public string Corridor { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
