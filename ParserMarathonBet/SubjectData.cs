using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserMarathonBet
{
    public class SubjectData
    {
        private string subjectName;
        private ObservableCollection<GroupData> groups;

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

        public ObservableCollection<GroupData> Groups
        {
            get => groups;
            set
            {
                if (groups != value)
                {
                    groups = value;
                    OnPropertyChanged(nameof(Groups));
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
