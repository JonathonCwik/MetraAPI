using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetraAPI.Models
{
    public class Line
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Station> Stations { get; set; }
    }
}
