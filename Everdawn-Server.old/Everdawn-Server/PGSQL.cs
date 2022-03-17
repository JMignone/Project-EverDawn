using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Everdawn_Server
{
    public class PGSQL
    {
        public static NpgsqlConnection connection;
        private static string[] con = { "localhost", "postgres" };
        // Connect to PostgreSQL database
        public static void connect(string host, string db)
        {
            try
            {
                string credFile = "Path/to/file";
                string[] creds = File.ReadAllText(credFile).Split(".");
                connection = new NpgsqlConnection($"Host={host};Username={creds[0]};Password={creds[1]};Database={db}");
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Disconnect
        public static void disconnect()
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static void createDatabase(string dbName)
        {
            try
            {
                new NpgsqlCommand($"CREATE DATABASE {dbName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Delete Database
        /*public static void deleteDatabase(string dbName) // If this function is anywhere yet, something is very wrong
        {
            try
            {
                new NpgsqlCommand($"DROP DATABASE IF EXISTS {dbName})").ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }*/
        public static void createTable(string tableName, Dictionary<string, List<string>> keyOptions)
        {
            try
            {
                Console.WriteLine(
                    $"Creating table: {tableName}\n" +
                    $"With the following keys and options:\n" +
                    $"{keyOptions}");
                string keys = "";
                foreach (KeyValuePair<string, List<string>> item in keyOptions)
                {
                    keys = keys == "" ? $"{item.Key} {item.Value}" : $"{keys}, {item.Key} {item.Value}";
                }
                int query = new NpgsqlCommand($"CREATE TABLE {tableName}({keys})", connection).ExecuteNonQuery() == -1 ? 1 : 0;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Delete Table
        /*public static void deleteTable(string tableName) // If this function is anywhere yet, something is very wrong
        {
            try
            {
                new NpgsqlCommand($"DROP TABLE IF EXISTS {tableName})").ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }*/
        // Query Table
        public static List<object> query(String query)
        {
            try
            {
                Console.WriteLine($"Running query: {query}");
                NpgsqlDataReader reader = new NpgsqlCommand(query, connection).ExecuteReader();
                int fieldCount = reader.FieldCount;
                List<object> list = new();
                while (reader.Read())
                {
                    Dictionary<string,object> fieldValues = new();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        fieldValues[reader.GetName(i)] = reader.GetValue(i);
                    }
                    list.Add(fieldValues);
                }
                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Insert Row
        public static void insertRow(string tableName, Dictionary<string, string> data)
        {
            try
            {
                string keys = String.Join(",", data.Keys.ToArray());
                string values = String.Join(",", data.Values.ToArray());
                new NpgsqlCommand($"INSERT INTO {tableName} ({keys}) VALUES ({values})", connection).ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Delete Row
        public static void deleteRow(string tableName, string[] condition)
        {
            try
            {
                if (condition.Length != 2)
                {
                    throw new ArgumentOutOfRangeException();
                }
                new NpgsqlCommand($"DELETE FROM {tableName} WHERE {condition[0]} = {condition[1]}").ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        // Startup
        public static void startup()
        {
            try
            {
                // Connect
                connect(con[0], con[1]);
                // Check if default schemas exist
                foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> db in Server.databases)
                {
                    Console.WriteLine($"Testing for database: {db.Key}");
                    var dbQuery = query(
                        $"SELECT EXISTS (" +
                        $"SELECT datname FROM pg_catalog.pg_database WHERE datname = '{db.Key}'" +
                        $")");
                    if (dbQuery[0].ToString() == "F" | dbQuery[0].ToString() == "False")
                    {
                        Console.WriteLine(
                            $"Database not found: {db.Key}");
                        createDatabase(db.Key);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Database found: {db.Key}");
                    }
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> table in db.Value)
                    {
                        Console.WriteLine($"Testing for table: {table.Key}");
                        var tbQuery = query(
                            $"SELECT EXISTS (" +
                            $"SELECT FROM pg_tables WHERE schemaname = '{db.Key}' AND tablename = '{table.Key}'" +
                            $")");
                        if (tbQuery[0].ToString() == "F" | tbQuery[0].ToString() == "False")
                        {
                            Console.WriteLine(
                                $"Table not found: {table.Key}");
                            createTable(table.Key, table.Value);
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Table found: {table.Key}");
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
