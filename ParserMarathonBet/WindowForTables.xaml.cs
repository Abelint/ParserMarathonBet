using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            dataGrid.ItemsSource = Display;
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(2)
            };
            timer.Tick += async (s, e) => await StartParseAsync();
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
            StartParseAsync();
            timer.Start();
        }

        private void RD()
        {
            Display.Clear();
            foreach (var r in Results)
            {
                ForDisplay forDisplay = new ForDisplay
                {
                    EventName = r.NodeName
                };
                bool color = true;
                Display.Add(forDisplay);
                if (r != null)
                {
                    for (int i = 1; i < r.TableContents.Count; i++)
                    {
                        var t = r.TableContents[i];
                        if (t != null)
                        {
                            color = !color;
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
                                Fora1 = t.Handicap1 + "\n" + t.HandicapGames1,
                                Fora2 = t.Handicap2 + "\n" + t.HandicapGames2,
                                Down = t.Under + "\n" + t.TotalGamesUnder,
                                Up = t.Over + "\n" + t.TotalGamesOver,
                                IsHighlighted = color
                            };
                            Display.Add(for2);
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
                var existingNode = Results.Count > i ? Results[i] : null;

                if (existingNode == null)
                {
                    Results.Add(newNode);
                }
                else
                {
                    existingNode.NodeName = newNode.NodeName;

                    for (int j = 0; j < newNode.TableContents.Count; j++)
                    {
                        var newEvent = newNode.TableContents[j];
                        var existingEvent = existingNode.TableContents.Count > j ? existingNode.TableContents[j] : null;

                        if (existingEvent == null)
                        {
                            existingNode.TableContents.Add(newEvent);
                        }
                        else if (!newEvent.Equals(existingEvent))
                        {
                            existingNode.TableContents[j] = newEvent;
                        }
                    }
                }
            }
        }
    }
}
