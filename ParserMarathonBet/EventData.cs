using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public class EventData
    {
        //public string EventName { get; set; }
        //public string Team1 { get; set; }
        //public string Team2 { get; set; }
        //public string Schet { get; set; }
        //public string Time { get; set; }
        //public Dictionary<string, string> EventColumns { get; set; }
        private string eventName;
        private string team1;
        private string team2;
        private string schet;
        private int schet1 = -1;
        private int schet2 = -1;
        private string time;
        private DateTime dateTime;
        private Dictionary<string, string> eventColumns;

        public string EventName
        {
            get => eventName;
            set
            {
                if (eventName != value)
                {
                    eventName = value;
                    OnPropertyChanged(nameof(EventName));
                }
            }
        }

        public string Team1
        {
            get => team1;
            set
            {
                if (team1 != value)
                {
                    team1 = value;
                    OnPropertyChanged(nameof(Team1));
                }
            }
        }

        public string Team2
        {
            get => team2;
            set
            {
                if (team2 != value)
                {
                    team2 = value;
                    OnPropertyChanged(nameof(Team2));
                }
            }
        }

        public string Schet
        {
            get => schet;
            set
            {
                if (schet != value)
                {
                    schet = value;
                    OnPropertyChanged(nameof(Schet));
                }
            }
        }

        public string Time
        {
            get => time;
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public Dictionary<string, string> EventColumns
        {
            get => eventColumns;
            set
            {
                if (eventColumns != value)
                {
                    eventColumns = value;
                  //  GetInfo();
                    OnPropertyChanged(nameof(EventColumns));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetInfo()
        {
            string pattern = @"^(\d+:\d+)";
            Match match = Regex.Match(schet, pattern);
            schet1 = Convert.ToInt32(match.Groups[1].Value.Split(':')[0]);
            schet2 = Convert.ToInt32(match.Groups[1].Value.Split(':')[1]);
            match = Regex.Match(Time, pattern);
            DateTime.TryParse(match.Groups[0].Value, out dateTime);
           

        }
    }
}
