﻿using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ParserMarathonBet
{
    public partial class WindowForTables : Window
    {
        public class NodeInfo
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string NodeName { get; set; }
            public ObservableCollection<EventInfo> TableContents { get; set; } = new ObservableCollection<EventInfo>();
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class EventInfo
        {
            public string EventName { get; set; }
            public string Time1 { get; set; }
            public string Player1 { get; set; }
            public string Time2 { get; set; }
            public string Player2 { get; set; }
            public DateTime DataParse { get; set; } = DateTime.Now;
            public string Odds1 { get; set; }
            public string Win1 { get; set; }
            public string Odds2 { get; set; }
            public string Win2 { get; set; }
            public string Handicap1 { get; set; }
            public string HandicapGames1 { get; set; }
            public string Handicap2 { get; set; }
            public string HandicapGames2 { get; set; }
            public string Under { get; set; }
            public string TotalGamesUnder { get; set; }
            public string Over { get; set; }
            public string TotalGamesOver { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is EventInfo other)
                {
                    return EventName == other.EventName &&
                           Time1 == other.Time1 &&
                           Player1 == other.Player1 &&
                           Time2 == other.Time2 &&
                           Player2 == other.Player2 &&
                           Odds1 == other.Odds1 &&
                           Win1 == other.Win1 &&
                           Odds2 == other.Odds2 &&
                           Win2 == other.Win2 &&
                           Handicap1 == other.Handicap1 &&
                           HandicapGames1 == other.HandicapGames1 &&
                           Handicap2 == other.Handicap2 &&
                           HandicapGames2 == other.HandicapGames2 &&
                           Under == other.Under &&
                           TotalGamesUnder == other.TotalGamesUnder &&
                           Over == other.Over &&
                           TotalGamesOver == other.TotalGamesOver;
                }
                return false;
            }

        }

        public class ForDisplay : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string eventName;
            private string time;
            private string dateParse;
            private string one;
            private string two;
            private string fora1;
            private string fora2;
            private string down;
            private string up;
            private bool isHighlighted;
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

            public string DataParse
            {
                get => dateParse;
                set
                {
                    if (dateParse != value)
                    {
                        dateParse = value;
                        OnPropertyChanged(nameof(DataParse));
                    }
                }
            }

            public string One
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

            public string Two
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

            public string Fora1
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

            public string Fora2
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

            public string Down
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

            public string Up
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
            public bool IsHighlighted
            {
                get => isHighlighted;
                set
                {
                    if (isHighlighted != value)
                    {
                        isHighlighted = value;
                        OnPropertyChanged(nameof(IsHighlighted));
                    }
                }
            }


            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<NodeInfo> Results = new ObservableCollection<NodeInfo>();
        public ObservableCollection<ForDisplay> Display = new ObservableCollection<ForDisplay>();
        private DispatcherTimer timer;

        public WindowForTables()
        {
            InitializeComponent();

            var data = AdapterClass.dBadapter.GetData("tennis");
            foreach (var item in data)
            {
                DateTime time = DateTime.Now;
                DateTime.TryParse((string)item["dateParse"], out time);
                  //private string eventName;
        //private string time;
        //private string dateParse;
        //private string one;
        //private string two;
        //private string fora1;
        //private string fora2;
        //private string down;
        //private string up;
        //private bool isHighlighted;
        ForDisplay tennisShow = new ForDisplay()
                {
                    EventName = (string)item["eventName"],
                    DataParse = (string)item["dateParse"],
                   
                   
                    Time = (string)item["time"],
                    One =(string)item["one"],
                    Two = (string)item["two"],                   
                    Fora1 =(string)item["fora1"],
                    Fora2 =(string)item["fora2"],
                    Down =(string)item["down"],
                    Up =(string)item["up"],
                    IsHighlighted = bool.Parse((string)item["isHighlighted"])
                   


                };
                TimeSpan timeSpan = time - DateTime.Now;

                if (timeSpan.TotalDays > 5) AdapterClass.dBadapter.DeleteData("tennis", $"id = {item["id"]}");
                else Display.Add(tennisShow);
            }



            dataGrid.ItemsSource = Display;
            FStartAsync();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };
            timer.Tick += async (s, e) => await StartParseAsync();
            timer.Start();
        }
        public async void FStartAsync()
        {
            await StartParseAsync();
        }
        public async Task StartParseAsync()
        {
            await Parse();
        }

        async Task Parse()
        {
            string baseUrl = "https://www.marathonbet.ru/su/betting/Tennis+-+2398?page=";
            int currentPage = 0;
            bool hasNextPage = true;

            HttpClient client = new HttpClient();

            var newResults = new ObservableCollection<NodeInfo>();

            while (hasNextPage)
            {
                string url = baseUrl + currentPage + "&pageAction=getPage";
                try
                {


                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Обработка ответа страницы
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(responseBody);

                    // Извлечение данных с текущей страницы
                    var nodes = document.DocumentNode.SelectNodes(".//div[contains(@class, 'category-container')]");
                    if (nodes == null || nodes.Count == 0)
                    {
                        hasNextPage = false;
                        continue;
                    }

                    nodes.RemoveAt(0); // Удаление первого узла

                    foreach (var node in nodes)
                    {
                        var nodeInfo = new NodeInfo();

                        // Ищем все таблицы с классом "category-header" внутри текущего узла
                        var groupNodes = node.SelectNodes(".//table[contains(@class, 'category-header')]");
                        if (groupNodes != null)
                        {
                            foreach (var groupNode in groupNodes)
                            {
                                var headerNode = groupNode.SelectSingleNode(".//h2[contains(@class, 'category-label')]");
                                if (headerNode != null)
                                {
                                    var eventText = headerNode.InnerText.Trim();
                                    nodeInfo.NodeName = eventText;
                                }
                            }
                        }

                        // Ищем все таблицы с классом "coupon-row-item" внутри текущего узла
                        var couponTables = node.SelectNodes(".//table[contains(@class, 'coupon-row-item')]");
                        if (couponTables != null)
                        {
                            for (int k = 0; k < couponTables.Count; k++)
                            {
                                var couponTable = couponTables[k];
                                var tableContent = couponTable.InnerText.Trim();

                                string[] parts = tableContent.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < parts.Length; i++)
                                {
                                    parts[i] = parts[i].Trim();
                                    if (parts[i] == "&mdash;")
                                    {
                                        parts[i] = "-";
                                    }
                                }
                                EventInfo info = new EventInfo();
                                if (k == 0) info = ParseEventInfo(parts, true);
                                else info = ParseEventInfo(parts, false);
                                nodeInfo.TableContents.Add(info);
                            }
                        }

                        newResults.Add(nodeInfo);
                    }

                    // Проверка, есть ли следующая страница
                    var hasNextPageNode = document.DocumentNode.SelectSingleNode("//script[contains(text(), 'hasNextPage')]");
                    if (hasNextPageNode != null)
                    {
                        string scriptContent = hasNextPageNode.InnerText;
                        int startIndex = scriptContent.IndexOf("{");
                        int endIndex = scriptContent.LastIndexOf("}");
                        if (startIndex != -1 && endIndex != -1)
                        {
                            string jsonResponse = scriptContent.Substring(startIndex, endIndex - startIndex + 1);
                            JObject json = JObject.Parse(jsonResponse);
                            hasNextPage = json["hasNextPage"]?.Value<bool>() ?? false;
                        }
                    }

                    if (hasNextPage)
                    {
                        currentPage++;
                    }
                }
                catch
                {
                    
                }
            }

            UpdateResults(newResults);

            // Для отладки: выводим результаты
            foreach (var nodeInfo in Results)
            {
                Console.WriteLine($"Node: {nodeInfo.NodeName}");
                foreach (var tableContent in nodeInfo.TableContents)
                {
                    Console.WriteLine($"  Table: {tableContent}");
                }
            }
            RD();
        }

        static EventInfo ParseEventInfo(string[] input, bool first)
        {
            if (input.Length == 17)
                return new EventInfo
                {
                    EventName = input[0] + input[1] + "\n" + input[3] + input[4],
                    Player1 = input[1],
                    Player2 = input[4],
                    Time1 = input[2],
                    Time2 = input[5],
                    Odds1 = input[1],
                    Win1 = input[7],
                    Odds2 = input[3],
                    Win2 = input[8],
                    Handicap1 = input[9],
                    HandicapGames1 = input[10],
                    Handicap2 = input[11],
                    HandicapGames2 = input[12],
                    Under = input[13],
                    TotalGamesUnder = input[14],
                    Over = input[15],
                    TotalGamesOver = input[16]
                };
            else if (input.Length == 13 && first)
            {
                return new EventInfo
                {
                    EventName = input[0],
                    Odds1 = input[1],
                    Win1 = input[2],
                    Odds2 = input[3],
                    Win2 = input[4],
                    Handicap1 = input[5],
                    HandicapGames1 = input[6],
                    Handicap2 = input[7],
                    HandicapGames2 = input[8],
                    Under = input[9],
                    TotalGamesUnder = input[10],
                    Over = input[11],
                    TotalGamesOver = input[12]
                };
            }
            else if (input.Length == 13)
            {
                return new EventInfo
                {
                    EventName = input[0] + input[1] + "\n" + input[3] + input[4],
                    Player1 = input[1],
                    Player2 = input[4],
                    Time1 = input[2],
                    Time2 = input[5],
                    Odds1 = input[1],
                    Win1 = input[6],
                    Odds2 = input[3],
                    Win2 = input[7],
                    Handicap1 = input[9],
                    HandicapGames1 = input[10],
                    Handicap2 = input[11],
                    HandicapGames2 = "-",
                    Under = "-",
                    TotalGamesUnder = "-",
                    Over = "-",
                    TotalGamesOver = "-"
                };
            }
            else if (input.Length == 14)
            {
                return new EventInfo
                {
                    EventName = input[0] + input[1] + "\n" + input[3] + input[4],
                    Player1 = input[1],
                    Player2 = input[4],
                    Time1 = input[2],
                    Time2 = input[5],
                    Odds1 = input[1],
                    Win1 = input[7],
                    Odds2 = input[3],
                    Win2 = input[8],
                    Handicap1 = input[9],
                    HandicapGames1 = input[10],
                    Handicap2 = input[11],
                    HandicapGames2 = input[12],
                    Under = "-",                    
                    TotalGamesUnder = "-",
                    Over = "-",
                    TotalGamesOver = "-"
                };
            }
            else if (input.Length == 12)
                return new EventInfo
                {
                    EventName = input[0] + input[1] + "\n" + input[3] + input[4],
                    Player1 = input[1],
                    Player2 = input[4],
                    Time1 = input[2],
                    Time2 = input[5],
                    Odds1 = input[1],
                    Win1 = input[6],
                    Odds2 = input[3],
                    Win2 = input[7],
                    Handicap1 = input[9],
                    HandicapGames1 = input[10],
                    Handicap2 = input[11],
                    HandicapGames2 = "-",
                    Under = "-",
                    TotalGamesUnder = "-",
                    Over = "-",
                    TotalGamesOver = "-"
                };
            else return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //StartParseAsync();
           
        }

        private void RD()
        {
            Display.Clear();
            var resultsTemp = new ObservableCollection<NodeInfo>(Results);
            // Временные списки для сортировки по категориям
            var newCollection = new List<NodeInfo>();
            var wtaSingles = new List<NodeInfo>();
            var atpSingles = new List<NodeInfo>();
            var wtaDoubles = new List<NodeInfo>();
            var atpDoubles = new List<NodeInfo>();
            var others = new List<ForDisplay>();

            string[] keys = { "WTA, одиночный разряд" , "ATP, одиночный разряд",
            "WTA, парный разряд","ATP, парный разряд"};
           
                foreach (var item in resultsTemp)
                {
                    if (item.NodeName.Contains("WTA") && item.NodeName.Contains("Одиночный разряд"))
                    {
                        wtaSingles.Add(item);                       
                    }
                }
            foreach (var item in wtaSingles) resultsTemp.Remove(item);


            foreach (var item in resultsTemp)
            {
                if (item.NodeName.Contains("ATP") && item.NodeName.Contains("Одиночный разряд"))
                {
                    atpSingles.Add(item);                   
                }
            }
            foreach (var item in atpSingles) resultsTemp.Remove(item);


            foreach (var item in resultsTemp)
            {
                if (item.NodeName.Contains("WTA") && item.NodeName.Contains("Парный разряд"))
                {
                    wtaDoubles.Add(item);                    
                }
            }
            foreach (var item in wtaDoubles) resultsTemp.Remove(item);

            foreach (var item in resultsTemp)
            {
                if (item.NodeName.Contains("ATP") && item.NodeName.Contains("Парный разряд"))
                {
                    atpDoubles.Add(item);                    
                }
            }
            foreach (var item in atpDoubles) resultsTemp.Remove(item);

            newCollection.AddRange(wtaSingles);
            newCollection.AddRange(atpSingles);
            newCollection.AddRange(wtaDoubles);
            newCollection.AddRange(atpDoubles);
            newCollection.AddRange(resultsTemp);

            foreach (var r in newCollection)
            {
                ForDisplay forDisplay = new ForDisplay
                {
                    EventName = r.NodeName
                };
                bool color = true;
                Display.Add(forDisplay);
                var data = new Dictionary<string, object>
                                {
                                    { AdapterClass.tennisColumn[0], forDisplay.EventName },
                                    { AdapterClass.tennisColumn[1], forDisplay.Time },
                                    { AdapterClass.tennisColumn[2], forDisplay.DataParse},
                                    { AdapterClass.tennisColumn[3], forDisplay.One },
                                    { AdapterClass.tennisColumn[4], forDisplay.Two},
                                    { AdapterClass.tennisColumn[5], forDisplay.Fora1},
                                    { AdapterClass.tennisColumn[6], forDisplay.Fora2},
                                    { AdapterClass.tennisColumn[7], forDisplay.Down},
                                    { AdapterClass.tennisColumn[8], forDisplay.Up},
                                    { AdapterClass.tennisColumn[9], forDisplay.IsHighlighted}
                                    
                                };
                AdapterClass.dBadapter.AddData("tennis", data);
                if (r != null)
                {
                    ForDisplay lastFD = null;
                    for (int i = 1; i < r.TableContents.Count; i++)
                    {
                        var t = r.TableContents[i];
                       
                        if (t != null)
                        {
                            if (lastFD != null)
                            {
                               if(lastFD.EventName != t.Player1 + "\n" + t.Player2) color = !color;
                            }
                            //color = !color;

                            string tstr = "";
                            if (t.Time1 != null)
                            {
                                string[] str = t.Time1.Split(' ');
                                if (str.Length == 3)
                                {
                                    tstr = str[0] + " " + str[1] + "\n" + str[2];
                                }
                                else tstr = t.Time1;
                            }
                            ForDisplay for2 = new ForDisplay
                            {
                                EventName = t.Player1 + "\n" + t.Player2,
                                Time = tstr,
                                DataParse = t.DataParse.ToString(),
                                One = t.Win1,
                                Two = t.Win2,
                                Fora1 = t.Handicap1 + " " + t.HandicapGames1,
                                Fora2 = t.Handicap2 + " " + t.HandicapGames2,
                                Down = t.Under + " " + t.TotalGamesUnder,
                                Up = t.Over + " " + t.TotalGamesOver,
                                IsHighlighted = color
                            };
                            lastFD = for2;

                            Display.Add(for2);
                            var data2 = new Dictionary<string, object>
                                {
                                    { AdapterClass.tennisColumn[0], for2.EventName },
                                    { AdapterClass.tennisColumn[1], for2.Time },
                                    { AdapterClass.tennisColumn[2], for2.DataParse},
                                    { AdapterClass.tennisColumn[3], for2.One },
                                    { AdapterClass.tennisColumn[4], for2.Two},
                                    { AdapterClass.tennisColumn[5], for2.Fora1},
                                    { AdapterClass.tennisColumn[6], for2.Fora2},
                                    { AdapterClass.tennisColumn[7], for2.Down},
                                    { AdapterClass.tennisColumn[8], for2.Up},
                                    { AdapterClass.tennisColumn[9], for2.IsHighlighted}

                                };
                            AdapterClass.dBadapter.AddData("tennis", data2);

                        }
                    }

                }
            }
        }
       

        private void UpdateResults(ObservableCollection<NodeInfo> newResults)
        {
            for (int i = 0; i < newResults.Count; i++)
            {
                var newNode = newResults[i];
                NodeInfo existingNode =  null;
                foreach (var exN in Results)
                {
                    if(exN.NodeName == newNode.NodeName) {
                        existingNode = exN;
                        break;  
                    }
                }
                if (existingNode == null)
                {
                    Results.Add(newNode);
                }
                else
                {
                    existingNode.NodeName = newNode.NodeName;

                    for (int j = 1; j < newNode.TableContents.Count; j++)
                    {
                        var newEvent = newNode.TableContents[j];
                        var existingEvent = existingNode.TableContents.Count > j ? existingNode.TableContents[j] : null;

                        if (existingEvent == null)
                        {
                            existingNode.TableContents.Add(newEvent);
                        }
                        else if (!newEvent.Equals(existingEvent))
                        {
                            //existingNode.TableContents[j] = newEvent;
                            DateTime nowTime = DateTime.Now;
                            var sobitiye = ParseDateString(newEvent.Time1);
                            TimeSpan timeUntilEvent = sobitiye - nowTime;
                            TimeSpan lastParse = newEvent.DataParse - nowTime;
                            int position = j + 1;
                            if (timeUntilEvent.TotalHours > 24 && lastParse.TotalHours > 3)
                            {
                                existingNode.TableContents.Insert(position, newEvent);
                            }
                            else if (timeUntilEvent.TotalHours <= 24 && timeUntilEvent.TotalHours > 1 &&
                                lastParse.TotalHours > 1)
                            {
                                existingNode.TableContents.Insert(position, newEvent);
                            }
                            else if (timeUntilEvent.TotalHours <= 1 && lastParse.TotalMinutes >= 5)
                            {
                                existingNode.TableContents.Insert(position, newEvent);
                            }
                            //existingNode.TableContents.Insert(position, newEvent);
                        }
                    }
                }
            }
        }
        static DateTime ParseDateString(string dateString)
        {
            string formatDate = "dd MMM HH:mm";
            string formatTime = "HH:mm";
            CultureInfo provider = new CultureInfo("ru-RU");

            DateTime result;

            // Try parsing full date and time format
            if (DateTime.TryParseExact(dateString, formatDate, provider, DateTimeStyles.None, out result))
            {
                return result;
            }
            // Try parsing only time format
            else if (DateTime.TryParseExact(dateString, formatTime, provider, DateTimeStyles.None, out result))
            {
                // If the input string only contains time, add today's date
                DateTime today = DateTime.Today;
                result = new DateTime(today.Year, today.Month, today.Day, result.Hour, result.Minute, result.Second);
                return result;
            }
            else
            {
                throw new FormatException("Invalid date format");
            }
        }
    }
}
