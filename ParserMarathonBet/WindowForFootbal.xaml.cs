using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ParserMarathonBet
{
    /// <summary>
    /// Логика взаимодействия для WindowForFootbal.xaml
    /// </summary>
    public partial class WindowForFootbal : Window
    {
        bool startParse = false;
        bool statusParse = false;
        private DispatcherTimer timerParser;
        private ObservableCollection<SubjectData> subjects = new ObservableCollection<SubjectData>();
        private ObservableCollection<FootbalShow> footbalShows= new ObservableCollection<FootbalShow>();
        public WindowForFootbal()
        {
            InitializeComponent();
            EventsDataGrid.ItemsSource = footbalShows;
            WindowForTables wft = new WindowForTables();
            wft.Show();

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            startParse = !startParse;
            if (startParse)
            {
                LoadButton.Content = "Остановить";
                timerParser = new DispatcherTimer();
                timerParser.Interval = TimeSpan.FromSeconds(3); // Set interval to 3 seconds
                timerParser.Tick += TimerParser_Tick;
                timerParser.Start();
            }
            else
            {
                LoadButton.Content = "Запуск";
                timerParser.Stop();
            }
        }
        private async void TimerParser_Tick(object sender, EventArgs e)
        {
            if (statusParse) return;
            statusParse = true;
            string[] url = UrlTextBox.Text.Split(';');
            foreach (var u in url)
            {
                await ParsePageAsync(u);
            }
            UpdateTable();
            statusParse = false;
        }
        private async Task ParsePageAsync(string url)
        {
            var httpClient = new HttpClient();

            try
            {
                var html = await httpClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var collapsedNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'category-container collapsed')]");
                List<string> ecids = new List<string>();

                if (collapsedNodes != null)
                {
                    foreach (var node in collapsedNodes)
                    {
                        var id = node.GetAttributeValue("id", string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            ecids.Add(id.Split('_')[1]);
                        }
                    }
                }

                if (ecids.Count > 0)
                {
                    string ecidsQuery = string.Join(",", ecids);
                    string newUrl = $"{url}?ecids={ecidsQuery}";
                    await ParsePageAsync(newUrl);
                    return;
                }

                var container = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-id='container_EVENTS']");

                if (container != null)
                {
                    var divBlocks = container.SelectNodes(".//div[contains(@class, 'sport-category-container')]");
                    if (divBlocks != null)
                    {
                        foreach (var div in divBlocks)
                        {
                            var header = div.SelectSingleNode(".//div[contains(@class, 'sport-category-header')]");
                            if (header != null)
                            {
                                var label = header.SelectSingleNode(".//a[contains(@class, 'sport-category-label')]");
                                if (label != null)
                                {
                                    string categoryName = label.InnerText.Trim();
                                    var subject = subjects.FirstOrDefault(s => s.SubjectName == categoryName);

                                    if (subject == null)
                                    {
                                        subject = new SubjectData { SubjectName = categoryName, Groups = new ObservableCollection<GroupData>() };
                                        subjects.Add(subject);
                                    }

                                    var nodes = div.SelectNodes(".//div[contains(@class, 'category-container')]");
                                    if (nodes != null)
                                    {
                                        foreach (var node in nodes)
                                        {
                                            var someData = node.InnerHtml.Trim().Replace("\n", "");
                                            var parser = new HtmlParser();
                                            var newGroups = parser.ParseHtml(div.InnerHtml);

                                            foreach (var newGroup in newGroups)
                                            {
                                                var existingGroup = subject.Groups.FirstOrDefault(g => g.GroupName == newGroup.GroupName);

                                                if (existingGroup == null)
                                                {
                                                    subject.Groups.Add(newGroup);
                                                }
                                                else
                                                {
                                                    existingGroup.Events = newGroup.Events;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Не удалось найти элементы по заданному XPath.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No div blocks found with data-sport-treeid='1372932'");
                    }
                }
                else
                {
                    Console.WriteLine("No container found with data-id='container_EVENTS'");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void UpdateTable()
        {
            foreach(var subject in subjects)
            {
                if (subject.SubjectName == "Киберспорт") continue;
                foreach (var group in subject.Groups)
                {
                    foreach (var ev in group.Events)
                    {
                        string[] str = { "-1", "-1", "-1" };
                        if (ev.Schet != null)  str = ev.Schet.Split(':');
                        int sch1 = Convert.ToInt32(str[0]);
                        int sch2 = Convert.ToInt32(str[1].Split(' ')[0]);
                        int tim = int.MinValue;
                        if (ev.Time != null)
                        {
                            str = ev.Time.Split(':');                            
                            int.TryParse(str[0], out tim);
                        }
                        string tempstr = ev.EventColumns["1"];
                        float on = TryParseFloat(ev.EventColumns["1"]);
                        
                        float tw = TryParseFloat(ev.EventColumns["2"]);
                        float x = TryParseFloat(ev.EventColumns["X"]);
                        float x1 = TryParseFloat(ev.EventColumns["1X"]);
                        float ot = TryParseFloat(ev.EventColumns["12"]);
                        float x2 = TryParseFloat(ev.EventColumns["X2"]);
                        str = ev.EventColumns["Фора1"].Trim().Split(' ');
                        float f1skoba = TryParseFloat(RemoveFirstAndLastCharacter(str[0]));
                        float f1 = float.NaN;
                        if (str.Length > 2)  f1 = TryParseFloat(str[2]);

                        str = ev.EventColumns["Фора2"].Trim().Split(' ');
                        float f2skoba = TryParseFloat(RemoveFirstAndLastCharacter(str[0]));
                        float f2 = float.NaN;
                        if (str.Length > 2)  f2 = TryParseFloat(str[2]);
                       

                        str = ev.EventColumns["Меньше"].Trim().Split(' ');
                        float downskoba = TryParseFloat(RemoveFirstAndLastCharacter(str[0]));
                        float down = float.NaN;
                        if (str.Length > 2)  down = TryParseFloat(str[2]);                                                
                       

                        str = ev.EventColumns["Больше"].Trim().Split(' ');
                        float upskoba = TryParseFloat(RemoveFirstAndLastCharacter(str[0]));
                        float up = float.NaN;
                        if (str.Length > 2)  up = TryParseFloat(str[2]);
                        
                      
                        var existingItem = footbalShows.FirstOrDefault(
                            fs => fs.Team1 == ev.Team1 && fs.Team2 == ev.Team2
                        );
                        if (existingItem != null)
                        {
                            if (Zadacha1(existingItem) || Zadacha2(existingItem) || Zadacha3(existingItem)
                                || Zadacha4(existingItem) || Zadacha5(existingItem) || Zadacha6(existingItem)
                                || Zadacha7(existingItem, tim, up))
                            {
                                // Обновление существующего элемента
                                existingItem.EventName = group.GroupName;
                                existingItem.Schet1 = sch1;
                                existingItem.Schet2 = sch2;
                                existingItem.Time = tim;
                                existingItem.One = on;
                                existingItem.Two = tw;
                                existingItem.X = x;
                                existingItem.X1 = x1;
                                existingItem.OneTwo = ot;
                                existingItem.X2 = x2;
                                existingItem.Fora1 = f1;
                                existingItem.Fora1skoba = f1skoba;
                                existingItem.Fora2 = f2;
                                existingItem.Fora2skoba = f2skoba;
                                existingItem.Down = down;
                                existingItem.Downskoba = downskoba;
                                existingItem.Upskoba = upskoba;
                                existingItem.Up = up;
                            }
                        }
                        else
                        {
                            // Добавление нового элемента
                            FootbalShow footbalShow2 = new FootbalShow
                            {
                                EventName = group.GroupName,
                                Team1 = ev.Team1,
                                Team2 = ev.Team2,
                                Schet1 = sch1,
                                Schet2 = sch2,
                                Time = tim,
                                One = on,
                                Two = tw,
                                X = x,
                                X1 = x1,
                                OneTwo = ot,
                                X2 = x2,
                                Fora1 = f1,
                                Fora2 = f2,
                                Down = down,
                                Up = up,
                                Fora1skoba= f1skoba,
                                Fora2skoba= f2skoba,
                                Downskoba = downskoba,
                                Upskoba = upskoba
                            };
                            if(Zadacha1(footbalShow2) || Zadacha2(footbalShow2)|| Zadacha3(footbalShow2)
                                || Zadacha4(footbalShow2)|| Zadacha5(footbalShow2)|| Zadacha6(footbalShow2))
                            footbalShows.Add(footbalShow2);
                        }
                       
                    }
                }
            }
        }

        float TryParseFloat(string str)
        {
            float temp = float.NaN;
            if (!float.TryParse(str, out temp))
            {
                float.TryParse(str.Replace('.', ','), out temp);
            }
            return temp;
        }
        public static string RemoveFirstAndLastCharacter(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 2)
            {
                // Если строка пустая или её длина меньше или равна 2, возвращаем пустую строку
                return string.Empty;
            }

            return input.Substring(1, input.Length - 2);
        }
        bool Zadacha1(FootbalShow footbalShow)
        {
            if (footbalShow.Time >= 70 && footbalShow.X > 1.99) return true;
            return false;
        }
        bool Zadacha2(FootbalShow footbalShow)
        {
            if (footbalShow.Time >= 70 && footbalShow.Schet1 == footbalShow.Schet2 && footbalShow.Fora1skoba==0 && footbalShow.Fora2skoba==0
                && (footbalShow.Fora1 <=1.28 || footbalShow.Fora2 <= 1.28)
                ) return true;
            return false;
        }
        bool Zadacha3(FootbalShow footbalShow)
        {
            if (footbalShow.Time >= 70 && Math.Abs(footbalShow.Fora1skoba) < Math.Abs(footbalShow.Schet1- footbalShow.Schet2)
                ) return true;
            return false;
        }
        bool Zadacha4(FootbalShow footbalShow)
        {
            if (footbalShow.Time >= 70 && footbalShow.Schet1 == footbalShow.Schet2 && (footbalShow.Fora1skoba != 0 || footbalShow.Fora2skoba != 0)
                ) return true;
            return false;
        }
        bool Zadacha5(FootbalShow footbalShow)
        {
            if (footbalShow.Time >=80 && footbalShow.Upskoba <=2
                ) return true;
            return false;
        }
        bool Zadacha6(FootbalShow footbalShow)
        {
            if (footbalShow.Time >= 85 && footbalShow.Upskoba <= 3
                ) return true;
            return false;
        }
        bool Zadacha7(FootbalShow footbalShow, float time, float up)
        {
            if (time >= 80 && up <= 1.07* footbalShow.Up
                ) return true;
            return false;
        }
    }
}
