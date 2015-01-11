using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class TrainCurrent
    {
        public string CorriderName { get; set; }
        public Station Station { get; set; }
        public int MITPositionM { get; set; }
        public int TrainNumber { get; set; }

    }
}
