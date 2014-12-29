using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetraAPI.Models
{
    public class Train
    {
        public DateTime EstimatedArvTime { get; set; }
        public DateTime EstimatedDptTime { get; set; }
        public bool HasData { get; set; }
        public bool HasDelay { get; set; }
        public bool IsRed { get; set; }
        public bool NotDeparted { get; set; }
        public DateTime ScheduledArvTime { get; set; }
        public DateTime ScheduledDptTime { get; set; }
        // Probably change to enum once I get what statuses are
        public int Status { get; set; }
        public long Timestamp { get; set; }
        public int TrainNum { get; set; }
        public string TripId { get; set; }
    }
}
