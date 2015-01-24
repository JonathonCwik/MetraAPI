using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class TrainData
    {
        public string DepartingStation { get; set; }
        public int TrainNumber { get; set; }
        public DateTime ScheduledDepartureTime { get; set; }
        public DateTime EstimatedDepartureTime { get; set; }
        public bool IsModified { get; set; }
        public DateTime Timestamp { get; set; }
        public int DateAge { get; set; }
        public bool IsDuplicate { get; set; }
        public int RunState { get; set; }
    }
}
