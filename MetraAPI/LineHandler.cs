using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetraAPI.Models;

namespace MetraAPI
{
    public class LineHandler
    {
        #region Lines
        private static List<Line> Lines = new List<Line>
        {
            new Line {
                Id = "UP-N",
                Name = "Union Pacific / North Line"
            },
            new Line {
                Id = "MD-N",
                Name = "Milwaukee District / North Line"
            },
            new Line {
                Id = "NCS",
                Name = "North Central Service"
            },
            new Line {
                Id = "UP-NW",
                Name = "Union Pacific / Northwest Line"
            },
            new Line {
                Id = "MD-W",
                Name = "Milwaukee District / West Line"
            },
            new Line {
                Id = "UP-W",
                Name = "Union Pacific / West Line"
            },
            new Line {
                Id = "BNSF",
                Name = "BNSF Railway"
            },
            new Line {
                Id = "HC",
                Name = "Heritage Corridor"
            },
            new Line {
                Id = "SWS",
                Name = "SouthWest Service"
            },
            new Line {
                Id = "RI",
                Name = "Rock Island District"
            },
            new Line {
                Id = "ME",
                Name = "Metra Electric District"
            },

        };
        #endregion

        public static List<Line> GetAllLines()
        {
            return Lines;
        }

        public static void UpdateAllStationsForAllLines()
        {
            foreach(var line in Lines)
            {
                UpdateStationsForLine(line);
            }
        }

        public static void UpdateStationsForLine(Line line)
        {
            if (Lines.Count(l => l.Id == line.Id) != 1)
            {
                throw new NullReferenceException(string.Format("Static lines do not contain a line with the id of {0}. Please try again.", line.Id));
            }

            Lines.Single(l => l.Id == line.Id).Stations = MetraAPI.GetStationsForLine(line);
        }
    }
}
