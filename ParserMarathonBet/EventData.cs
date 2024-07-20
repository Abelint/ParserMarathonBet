using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public class EventData
    {
        public string EventName { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public Dictionary<string, string> EventColumns { get; set; }
    }
}
