using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace ParserMarathonBet
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<GroupData> groups = new ObservableCollection<GroupData>();

        public MainWindow()
        {
            InitializeComponent();
            GroupsListBox.ItemsSource = groups;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
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

                // Clear the list box and add new items
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'category-container')]");
                if (nodes != null)
                {
                   
                    int count = 0;
                    foreach (var node in nodes)
                    {
                       // var someData = node.InnerText.Trim().Replace("\n", "");
                        var someData = node.InnerHtml.Trim().Replace("\n", "");
                       
                        count++;
                       
                        if (count > 1) Show(someData);
                       
                    }
                    
                }
                else
                {
                    MessageBox.Show("Не удалось найти элементы по заданному XPath.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Show(string html)
        {

            //var parser = new HtmlParser();
            var parser = new HtmlParser2();
            foreach (var p in parser.ParseHtml(html))
            {
                groups.Add(p);
            }
            

            
        }

        private void GroupsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (GroupsListBox.SelectedItem is GroupData selectedGroup)
            {
                EventsDataGrid.ItemsSource = selectedGroup.Events;
                Dictionary<string, string> tempDict = selectedGroup.Events[0].EventColumns;
            }
        }
    }
}
