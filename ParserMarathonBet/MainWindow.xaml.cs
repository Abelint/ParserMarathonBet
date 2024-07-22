using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using HtmlAgilityPack;

namespace ParserMarathonBet
{
    public partial class MainWindow : Window
    {
     

        private ObservableCollection<SubjectData> subjects = new ObservableCollection<SubjectData>();

        public MainWindow()
        {
            InitializeComponent();
            // GroupsListBox.ItemsSource = groups;
            SubjectsListBox.ItemsSource = subjects;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            subjects.Clear();
            string url = UrlTextBox.Text;
            await ParsePageAsync(url);
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

                // If there are any collapsed nodes, add their ids to the query and parse again
                if (ecids.Count > 0)
                {
                    string ecidsQuery = string.Join(",", ecids);
                    string newUrl = $"{url}?ecids={ecidsQuery}";
                    await ParsePageAsync(newUrl);
                    return;  // Return early since we're parsing the new URL
                }

                // Найти контейнер с data-id="container_EVENTS"
                var container = htmlDoc.DocumentNode
                    .SelectSingleNode("//div[@data-id='container_EVENTS']");

                if (container != null)
                {
                    // Найти все div внутри контейнера с data-sport-treeid="1372932"
                    var divBlocks = container
                        .SelectNodes(".//div[contains(@class, 'sport-category-container')]");

                    if (divBlocks != null)
                    {
                        foreach (var div in divBlocks)
                        {
                            var header = div.SelectSingleNode(".//div[contains(@class, 'sport-category-header')]");
                            SubjectData subject = new SubjectData();
                            if (header != null)
                            {
                                // Найти элемент с классом sport-category-label и получить его текст
                                var label = header.SelectSingleNode(".//a[contains(@class, 'sport-category-label')]");
                                if (label != null)
                                {
                                   string categoryName = label.InnerText.Trim();
                                   subject.SubjectName = categoryName;
                                }
                            }

                            // Clear the list box and add new items
                            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'category-container')]");
                            if (nodes != null)
                            {
                                subject.Groups = new List<GroupData>();
                                int count = 0;
                                foreach (var node in nodes)
                                {
                                    // var someData = node.InnerText.Trim().Replace("\n", "");
                                    var someData = node.InnerHtml.Trim().Replace("\n", "");

                                    count++;
                                    var parser = new HtmlParser2();
                                    subject.Groups = parser.ParseHtml(html);   

                                }

                            }
                            else
                            {
                                MessageBox.Show("Не удалось найти элементы по заданному XPath.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            subjects.Add(subject);
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
                var events = selectedGroups.SelectMany(g => g.Events).Distinct().ToList();
                EventsDataGrid.ItemsSource = events;
                UpdateEventColumns(events);
            }
            else
            {
                EventsDataGrid.ItemsSource = null;
            }
        }

        private void UpdateEventColumns(List<EventData> events)
        {
            EventsDataGrid.Columns.Clear();

            EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "EventName", Binding = new Binding("EventName") });
            EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Team1", Binding = new Binding("Team1") });
            EventsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Team2", Binding = new Binding("Team2") });

            if (events.Any())
            {
                var allKeys = events.SelectMany(e => e.EventColumns.Keys).Distinct().ToList();
                foreach (var key in allKeys)
                {
                    EventsDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = key,
                        Binding = new Binding($"EventColumns[{key}]")
                    });
                }
            }
        }

      
    }
}
