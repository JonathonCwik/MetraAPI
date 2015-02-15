using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class TrainSchedule
    {
        public string CorridorName { get; set; }
        public string StationName { get; set; }
        public float MITPositionM { get; set; }
        public int TrainNumber { get; set; }
        public int Direction { get; set; }
        public DateTime StopTime { get; set; }
        public DateTime EstStopTime { get; set; }
        public bool WasModified { get; set; }
        public string StopType { get; set; }
        public TrainStation Station { get; set; }
        public Line Line { get; set; }
        public bool IsDuplicate { get; set; }
        public int RunState { get; set; }
    }
}
