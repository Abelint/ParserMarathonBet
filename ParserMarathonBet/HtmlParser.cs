using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserMarathonBet
{
    public class HtmlParser
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
                    Events = new System.Collections.ObjectModel.ObservableCollection<EventData>() 
                };
                var temp = ParseHtmlTable(groupNode);
                string[] keys = {  "1", "X", "2", "1X", "12", "X2", "Фора1", "Фора2", "Меньше", "Больше" };
                // Находим блоки событий
                var eventNodes = groupNode.SelectSingleNode(".//following-sibling::div[contains(@class, 'category-content')]")
                    .SelectNodes(".//div[contains(@class, 'bg coupon-row')]");
               
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

                    var schet = eventNode.SelectSingleNode(".//div[contains(@class, 'cl-left')]");
                    string tempTime = "-";
                    // Извлекаем текст с результатом и временем
                    if (schet != null)
                    {
                        eventData.Schet = schet.InnerText.Trim().Split('\n')[0];
                        var eventTimeNode = schet.SelectSingleNode(".//div[@class='green bold nobr']");
                        tempTime = eventTimeNode?.InnerText.Trim();
                    }



                    eventData.Time = tempTime;


                    // Извлекаем данные столбцов
                    var columns = eventNode.SelectNodes(".//td[contains(@class, 'price')]");
                    int count = 0;
                    int countKey =0;
                    foreach (var column in columns)
                    {
                        //var key = column.SelectSingleNode(".//span").InnerText.Trim();
                        var key = "";
                        if (countKey < keys.Length) key = keys[countKey];
                        countKey++;
                        var value = "-";
                        if (column.SelectSingleNode(".//span") != null)
                        {
                            value = column.SelectSingleNode(".//span").InnerText.Trim();
                            if (value == "&mdash;") value = "-";
                        }
                            
                        //if(column.SelectSingleNode(".//span[contains(@class, 'selection-linkactive-selection')]")!=null)
                        //    value = column.SelectSingleNode(".//span[contains(@class, 'selection-linkactive-selection')]").InnerText.Trim();
                        eventData.EventColumns[key] = value;
                        count++;
                    }

                    groupData.Events.Add(eventData);
                }

                groups.Add(groupData);
            }

            return groups;
        }

        List<string>  ParseHtmlTable(HtmlNode html)
        {
           
            List<string> columnNames = new List<string>();
            // Ищем таблицу по классу
            var table = html.SelectSingleNode("//table[@class='coupon-row-item coupone-labels']");

            if (table != null)
            {
                // Ищем строки таблицы
                var rows = table.SelectNodes(".//tr");

                foreach (var row in rows)
                {
                    // Ищем ячейки в строке
                    var cells = row.SelectNodes(".//th | .//td");

                    if (cells != null)
                    {
                        var cellValues = cells.Select(cell => cell.InnerText.Trim()).ToList();
                        columnNames.AddRange(cellValues);
                        var result = string.Join("; ", cellValues);
                        Console.WriteLine(result);
                       
                    }
                }
            }
            else
            {
                Console.WriteLine("Таблица не найдена.");
            }
            return columnNames;
        }


        List<string> ParseSecondTable(string html)
        {
            List<string> strings = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Ищем таблицу по классу
            var table = doc.DocumentNode.SelectSingleNode("//table[@class='coupon-row-item']");

            if (table != null)
            {
                // Ищем строки таблицы
                var rows = table.SelectNodes(".//tr[@class='sub-row']");

                foreach (var row in rows)
                {
                    // Извлекаем данные о событии и времени
                    var eventData = row.SelectSingleNode(".//div[@class='cl-left red']").InnerText.Trim();
                    var eventTime = row.SelectSingleNode(".//div[@class='green bold nobr']").InnerText.Trim();

                    Console.WriteLine(eventData);
                    Console.WriteLine(eventTime);

                    // Ищем ячейки с коэффициентами
                    var priceCells = row.SelectNodes(".//td[contains(@class, 'price')]//span[@class='selection-link']");

                    if (priceCells != null)
                    {
                        var priceValues = priceCells.Select(cell => cell.InnerText.Trim()).ToList();
                        strings = priceValues;
                        var result = string.Join("\n", priceValues);
                        Console.WriteLine(result);
                    }
                }
            }
            else
            {
                Console.WriteLine("Таблица не найдена.");
            }
            return strings;
        }
    }
}
