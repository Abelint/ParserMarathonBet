using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public static class AdapterClass
    {
       
        public static DBadapter dBadapter = new DBadapter();
        public static string[] footbalColumn = { "eventName", "team1","dateParse",
            "team2", "schet1", "schet2",
             "time", "one", "two", "x", "x1", "oneTwo","x2","fora1","fora2","down",
            "up",
            "fora1skoba","fora2skoba","downskoba","upskoba"};

        public static string[] tennisColumn = { "eventName", "time","dateParse", 
            "one", "two","fora1","fora2","down","up",
            "isHighlighted"};
    }
}
