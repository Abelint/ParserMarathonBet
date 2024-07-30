using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public class Tennis
    {
        

        public string Name { get; set; }
        public DateTime TimeStart { get; set; }
        public string GetTimeStart
        {
            get
            {
                string str = TimeStart.Date.ToString() + "\n" + TimeStart.TimeOfDay;
                return str;
            }
        }
        public DateTime TimeParse { get; set;}
        public string GetTimeParse
        {
            get
            {
                string str = TimeParse.Date.ToString()+"\n" + TimeParse.TimeOfDay;
                return str;
            }
        }
        public string One { get { return one.ToString(); } }
        public float one;        
        public string Two { get { return two.ToString(); } }
        public float two;
        public string ForaOne
        {
            get
            {
                string str = "";
                if (foraOneUpString != float.MinValue)
                {
                    str = "(" + foraOneUpString + ")\n" + foraOneDownString;
                }
                return str;
            }
        }
        public float foraOneUpString = float.MinValue;
        public float foraOneDownString = float.MinValue;
        public string ForaTwo { get {
                string str = "";
                if(foraTwoUpString != float.MinValue)
                {
                    str = "("+ foraTwoUpString+")\n" + foraTwoDownString;
                }
                return str;
            }  }
        public float foraTwoUpString = float.MinValue;
        public float foraTwoDownString = float.MinValue;
        public string Lower
        {
            get
            {
                string str = "";
                if (lowerUpString != float.MinValue)
                {
                    str = "(" + lowerUpString + ")\n" + lowerDownString;
                }
                return str;
            }
        }
        public float lowerUpString = float.MinValue;
        public float lowerDownString = float.MinValue;
        public string Greater
        {
            get
            {
                string str = "";
                if (GreaterUpString != float.MinValue)
                {
                    str = "(" + GreaterUpString + ")\n" + GreaterDownString;
                }
                return str;
            }
        }
        public float GreaterUpString = float.MinValue;
        public float GreaterDownString = float.MinValue;


    }
}
