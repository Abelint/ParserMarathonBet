using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace ParserMarathonBet
{
    public class DBadapter
    {
        private string _dbName = "MarathonBet";
        private List<string> tables = new List<string>();
        private SQLiteConnection qLiteConnection = null;

        /// <summary>
        /// Список таблиц в базе данных
        /// </summary>
        public string[] Tables
        {
            get
            {
                return tables.ToArray();
            }
        }

        public DBadapter() { }
        /// <summary>
        /// Создание экземпляра с параметрами
        /// </summary>
        /// <param name="name">имя БД</param>
        public DBadapter(string name)
        {
            _dbName = name;
        }
        public void Init()
        {
            CreateDB(_dbName);
            string connectionString = "Data Source=" + _dbName + ".sqlite;Version=3;";
            qLiteConnection = new SQLiteConnection(connectionString);
        }
        /// <summary>
        /// Создание таблиц по шаблону
        /// </summary>
        /// <param name="pairs">шаблон название - столбцы</param>
        public void CreateTables(Dictionary<string, string[]> pairs)
        {
            foreach (var pair in pairs)
            {
                CreateTableDB(qLiteConnection, pair.Key, pair.Value);
            }

        }
        public void AddData(string tableName, Dictionary<string, object> data)
        {
            qLiteConnection.Open();

            var columns = string.Join(", ", data.Keys);
            var values = string.Join(", ", data.Values.Select(value => $"'{value}'"));
            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            SQLiteCommand command = new SQLiteCommand(sql, qLiteConnection);
            command.ExecuteNonQuery();

            qLiteConnection.Close();
        }
        public void ReplaceData(string tableName, Dictionary<string, object> data, string condition)
        {
            qLiteConnection.Open();

            var setClause = string.Join(", ", data.Select(kv => $"{kv.Key} = '{kv.Value}'"));
            var sql = $"UPDATE {tableName} SET {setClause} WHERE {condition}";

            SQLiteCommand command = new SQLiteCommand(sql, qLiteConnection);
            command.ExecuteNonQuery();

            qLiteConnection.Close();
        }

        public void DeleteData(string tableName, string condition)
        {
            qLiteConnection.Open();

            var sql = $"DELETE FROM {tableName} WHERE {condition}";

            SQLiteCommand command = new SQLiteCommand(sql, qLiteConnection);
            command.ExecuteNonQuery();

            qLiteConnection.Close();
        }

        public List<Dictionary<string, object>> GetData(string tableName, string condition = "")
        {
            qLiteConnection.Open();

            var sql = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrEmpty(condition))
            {
                sql += $" WHERE {condition}";
            }

            SQLiteCommand command = new SQLiteCommand(sql, qLiteConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            var results = new List<Dictionary<string, object>>();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }

            qLiteConnection.Close();

            return results;
        }


        private static void CreateDB(string name)
        {
            if (!File.Exists(name + ".sqlite")) SQLiteConnection.CreateFile(name + ".sqlite");
        }
        private void CreateTableDB(SQLiteConnection m_dbConnection, string tableName, string[] columns)
        {

            m_dbConnection.Open();
            string sql = "CREATE TABLE IF NOT EXISTS " + tableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT,";     //name varchar(20), score int)";
            for (int i = 0; i < columns.Length; i++)
            {
                if (i != columns.Length - 1) sql += columns[i] + ", ";
                else sql += columns[i];
            }
            sql += ")";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            m_dbConnection.Close();



        }


    }
}
