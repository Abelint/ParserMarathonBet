using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Documents;

namespace ParserMarathonBet
{
    public class HtmlParser2
    {
        public List<GroupData> ParseHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var groups = new List<GroupData>();

            // Ищем все таблицы с классом "category-header"
            var groupNodes = doc.DocumentNode.SelectNodes("//table[contains(@class, 'category-header')]");

            foreach (var groupNode in groupNodes)
            {
                var groupName = groupNode.SelectSingleNode(".//h2").InnerText.Trim();
                var groupData = new GroupData
                {
                    GroupName = groupName,
                    Events = new List<EventData>()
                };

                //var keys = groupNode.SelectNodes(".//table[contains(@class, 'coupon-row-item coupone-labels')]");
                //var key2 = keys[0].GetAttributeValue("class", "").Trim();
                //var valueKey = keys[0].SelectSingleNode(".//span[contains(@class, 'hint')]");

                // Находим блоки событий
                var eventNodes = groupNode.SelectSingleNode(".//following-sibling::div[contains(@class, 'category-content')]")
                    .SelectNodes(".//div[contains(@class, 'bg coupon-row')]");

                
                string[] keys = { "Название события", "", "1", "X", "2", "1X", "12", "X2", "Фора1", "Фора2", "Меньше", "Больше" }  ;

                foreach (var eventNode in eventNodes)
                {
                    var eventName = eventNode.GetAttributeValue("data-event-name", string.Empty).Trim();
                    var teams = eventNode.SelectNodes(".//div[contains(@class, 'member-name')]//span")
                                         .Select(n => n.InnerText.Trim()).ToList();

                    var eventData = new EventData
                    {
                        EventName = eventName,
                        Team1 = teams[0],
                        Team2 = teams[1],
                        EventColumns = new Dictionary<string, string>()
                    };

                 
                    // Извлекаем данные столбцов
                    var columns = eventNode.SelectNodes(".//td[contains(@class, 'price')]");
                    int count = 0;
                    int countKey =0;
                    foreach (var column in columns)
                    {
                        //var key = column.SelectSingleNode(".//span").InnerText.Trim();
                        var key = "";
                        if(countKey<keys.Length)key = keys[countKey];
                        countKey++;
                        var value = "-";
                        if (column.SelectSingleNode(".//span[contains(@class, 'selection-linkactive-selection')]") != null)
                            value = column.SelectSingleNode(".//span[contains(@class, 'selection-linkactive-selection')]").InnerText.Trim();
                        eventData.EventColumns[key] = value;
                        count++;
                    }

                    groupData.Events.Add(eventData);
                }

                groups.Add(groupData);
            }

            return groups;
        }
    }
}
