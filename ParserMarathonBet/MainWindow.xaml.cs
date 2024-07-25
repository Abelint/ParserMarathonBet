using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Xml;
using HtmlAgilityPack;

namespace ParserMarathonBet
{
    public partial class MainWindow : Window
    {
     

        private ObservableCollection<SubjectData> subjects = new ObservableCollection<SubjectData>();
        List<EventData> events = new List<EventData>();
        bool start = false;
        bool startStatus = false;
        bool startParse = false; 
        bool statusParse = false;
        private DispatcherTimer timer;
        private DispatcherTimer timerParser;

        public MainWindow()
        {
            InitializeComponent();
            // GroupsListBox.ItemsSource = groups;
            SubjectsListBox.ItemsSource = subjects;

            // Initialize and start the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Set interval to 30 seconds
            timer.Tick += Timer_Tick;
            timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (start)
            {
                if(startStatus) return; 
                startStatus = true;
                if (GroupsListBox.SelectedItems.Count > 0)
                {
                    var selectedGroups = GroupsListBox.SelectedItems.Cast<GroupData>().ToList();
                    events = selectedGroups.SelectMany(g => g.Events).Distinct().ToList();
                    EventsDataGrid.ItemsSource = events;
                    //UpdateEventColumns(events);
                    start = true;

                }
                else
                {
                    EventsDataGrid.ItemsSource = null;
                }


                EventsDataGrid.Columns.Clear();

                EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "EventName", Binding = new Binding("EventName") });
                EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Team1", Binding = new Binding("Team1") });
                EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Team2", Binding = new Binding("Team2") });
                EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Счет", Binding = new Binding("Schet") });
                EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Время", Binding = new Binding("Time") });

                if (events.Any())
                {
                    var allKeys = events.SelectMany(events => events.EventColumns.Keys).Distinct().ToList();
                    foreach (var key in allKeys)
                    {
                        EventsDataGrid.Columns.Add(new DataGridTextColumn
                        {
                            Header = key,
                            Binding = new Binding($"EventColumns[{key}]")
                        });
                    }
                }
                startStatus = false;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            startParse = !startParse;
            if (startParse)
            {
                LoadButton.Content = "Остановить";
                timerParser = new DispatcherTimer();
                timerParser.Interval = TimeSpan.FromSeconds(3); // Set interval to 30 seconds
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
                            ecids.Add(id.Split('_')[1]);  // Assuming the ID format is always 'container_<number>'
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
                                    //else
                                    //{
                                    //    CheckSubject(subject);
                                        
                                    //}
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
                                                   // UpdateEventColumns(existingGroup.Events.ToList());
                                                   // subject.Groups.Add(existingGroup);

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
               // MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CheckSubject(SubjectData subject)
        {
            if (subject == null) { return; }
            bool update = false;
            for(int i = 0; i < subjects.Count; i++) {
                if (subject.SubjectName == subjects[i].SubjectName)
                {
                   for(int j = 0; j < subjects[i].Groups.Count; j++)
                    {
                        for (int k = 0;subject.Groups.Count > k; k++)
                        {
                            if (subjects[i].Groups[j].GroupName == subject.Groups[k].GroupName)
                            {
                                update = true;
                                subjects[i].Groups[j]= subject.Groups[k];
                                
                            }
                        }
                        
                    }
                }
            }
            if(!update) { subjects.Add(subject); }

        }
     

        private void SubjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectsListBox.SelectedItems.Count > 0)
            {
                var selectedSubjects = SubjectsListBox.SelectedItems.Cast<SubjectData>().ToList();
                var groups = selectedSubjects.SelectMany(s => s.Groups).Distinct().ToList();
                GroupsListBox.ItemsSource = groups;
            }
            else
            {
                GroupsListBox.ItemsSource = null;
                EventsDataGrid.ItemsSource = null;
            }
        }

        private void GroupsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsListBox.SelectedItems.Count > 0)
            {
                var selectedGroups = GroupsListBox.SelectedItems.Cast<GroupData>().ToList();
                events = selectedGroups.SelectMany(g => g.Events).Distinct().ToList();
                EventsDataGrid.ItemsSource = events;
                
                start = true;
               
               

            }
            else
            {
                EventsDataGrid.ItemsSource = null;
            }
        }

     

      
    }
}
