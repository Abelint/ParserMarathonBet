using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public class FootbalShow : INotifyPropertyChanged
    
    {
        private string subjectName;
        public string SubjectName
        {
            get => subjectName;
            set
            {
                if (subjectName != value)
                {
                    subjectName = value;
                    OnPropertyChanged(nameof(SubjectName));
                }
            }
        }
        private string groupName;

        public string GroupName
        {
            get => groupName;
            set
            {
                if (groupName != value)
                {
                    groupName = value;
                    OnPropertyChanged(nameof(GroupName));
                }
            }
        }
        private string eventName;
        private string team1;
        private string team2;
        private int schet1 = -1;
        private int schet2 = -1;
        private int time; // только минуты
        private float one = float.NaN;
        private float two = float.NaN;
        private float x = float.NaN;
        private float x1 = float.NaN;
        private float oneTwo = float.NaN;
        private float x2 = float.NaN;
        private float fora1 = float.NaN;
        private float fora2 = float.NaN;
        private float down = float.NaN;
        private float up = float.NaN;

        private float fora1skoba = float.NaN;
        private float fora2skoba = float.NaN;
        private float downskoba = float.NaN;
        private float upskoba = float.NaN;

        private string fora1_All = "-";
        private string fora2_All = "-";
        private string down_All = "-";
        private string up_All = "-";
        public string Fora1_All
        {
            get => "(" + fora1skoba+")"+ "\n"+fora1;
           
        }
        public string Fora2_All
        {
            get => "(" + fora2skoba + ")" + "\n" + fora2;
           
        }
        public string Down_All
        {
            get => "(" + downskoba + ")" + "\n" + down;           
        }
        public string Up_All
        {
            get => "(" + downskoba + ")" + "\n" + down;
           
        }
        public float Upskoba
        {
            get => upskoba;
            set
            {
                if (upskoba != value)
                {
                    upskoba = value;
                    OnPropertyChanged(nameof(Upskoba));
                }
            }
        }
        public float Downskoba
        {
            get => downskoba;
            set
            {
                if (downskoba != value)
                {
                    downskoba = value;
                    OnPropertyChanged(nameof(Downskoba));
                }
            }
        }
        public float Fora2skoba
        {
            get => fora2skoba;
            set
            {
                if (fora2skoba != value)
                {
                    fora2skoba = value;
                    OnPropertyChanged(nameof(Fora2skoba));
                }
            }
        }
        public float Fora1skoba
        {
            get => fora1skoba;
            set
            {
                if (fora1skoba != value)
                {
                    fora1skoba = value;
                    OnPropertyChanged(nameof(Fora1skoba));
                }
            }
        }
        public string Teams { get { return team1 + "\n" + team2; } }
        public string Schet { get
            {
                if(schet1 != -1 && schet2!=-1) return schet1 + ":" + schet2;
                else return "";
            } }
        public float Up
        {
            get => up;
            set
            {
                if (up != value)
                {
                    up = value;
                    OnPropertyChanged(nameof(Up));
                }
            }
        }
        public float Down
        {
            get => down;
            set
            {
                if (down != value)
                {
                    down = value;
                    OnPropertyChanged(nameof(Down));
                }
            }
        }
        public float Fora2
        {
            get => fora2;
            set
            {
                if (fora2 != value)
                {
                    fora2 = value;
                    OnPropertyChanged(nameof(Fora2));
                }
            }
        }
        public float Fora1
        {
            get => fora1;
            set
            {
                if (fora1 != value)
                {
                    fora1 = value;
                    OnPropertyChanged(nameof(Fora1));
                }
            }
        }
        public float X2
        {
            get => x2;
            set
            {
                if (x2 != value)
                {
                    x2 = value;
                    OnPropertyChanged(nameof(X2));
                }
            }
        }
        public float OneTwo
        {
            get => oneTwo;
            set
            {
                if (oneTwo != value)
                {
                    oneTwo = value;
                    OnPropertyChanged(nameof(OneTwo));
                }
            }
        }
        public float X1
        {
            get => x1;
            set
            {
                if (x1 != value)
                {
                    x1 = value;
                    OnPropertyChanged(nameof(X1));
                }
            }
        }
        public float X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        public float Two
        {
            get => two;
            set
            {
                if (two != value)
                {
                    two = value;
                    OnPropertyChanged(nameof(Two));
                }
            }
        }
        public float One
        {
            get => one;
            set
            {
                if (one != value)
                {
                    one = value;
                    OnPropertyChanged(nameof(One));
                }
            }
        }
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

        public int Schet1
        {
            get => schet1;
            set
            {
                if (schet1 != value)
                {
                    schet1 = value;
                    OnPropertyChanged(nameof(Schet1));
                }
            }
        }
        public int Schet2
        {
            get => schet2;
            set
            {
                if (schet2 != value)
                {
                    schet2 = value;
                    OnPropertyChanged(nameof(Schet2));
                }
            }
        }
        public int Time
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
