using System.Collections;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Data.Sqlite;

namespace SQLITEAPI
{
    static public class Interface 
    {
        // In here is where all the magic happens, maybe I'll separate the methods to a different static class later.
        private static string? query;
        public static string? GetValueResult;


        static public string[] DatabaseCommandHelp =
        {
            "Create, DatabaseName",
            "Delete, DatabaseName",
            "Select, DatabaseName"
        };
        static public string[] TableCommandHelp =
        {
            "Create, TableName, Specifications",
            "TableDestroy, TableName",
            "AddValue, TableName, Column, Value",
            "ViewTable, TableName"
        };
        static string? connectionString;
        static string? selectedDatabase; 


        public static void ShowHelp(string[] commandList) 
        {
            System.Console.WriteLine("Comandos: \n");

            foreach (string command in commandList)
            {
                System.Console.WriteLine(command);
            }
        }

        public static void CheckIfDatabaseExists(string filepath)
        {
            if (!File.Exists($"{filepath}.db"))
            {
                throw new Exception("Arquivo não existe");
            }
        }

        public static void DatabaseCommand(params string[] args)
        {
            switch(args[0])
            {
                case commandsDefinitions.databaseCommands.Create:
                    using (FileStream fs = File.Create($@"SQLITEAPI\{args[1]}.db"))
                    {
                        // Ensure the file is created and closed properly
                    }
                    break;
                case commandsDefinitions.databaseCommands.Delete:
                    CheckIfDatabaseExists($@"SQLITEAPI\{args[1]}.db");
                    File.Delete($@"SQLITEAPI\{args[1]}.db");
                    break;
                case commandsDefinitions.databaseCommands.Select:
                    File.WriteAllText(@"SQLITEAPI\selectedDatabase.txt", args[1]);
                    selectedDatabase = $@"SQLITEAPI\{args[1]}.db";
                    connectionString = $@"Data Source={selectedDatabase};";
                    System.Console.WriteLine($"Connection string set to: {connectionString}");
                    break;
                default:
                    ShowHelp(DatabaseCommandHelp);
                    break;
            }
        }

        public static void TableCommand(params string[] args)
        {
            if (connectionString == null)
            {
                throw new Exception("Connection string is null. Please select a database first.");
            }

            if (args == null || args.Length == 0)
            {
                ShowHelp(TableCommandHelp);
                return;
            }

            System.Console.WriteLine($"Opening connection with connection string: {connectionString}");
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try 
                {
                    conn.Open();
                    System.Console.WriteLine("Connection opened successfully.");
                    switch (args[0])
                    {
                        case commandsDefinitions.TableCommands.Create:
                            if (args.Length < 3)
                            {
                                throw new ArgumentException("Table creation requires table name and specifications.");
                            }
                            query = $"CREATE TABLE {args[1]} ({args[2]})";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                comm.ExecuteNonQuery();
                            }
                            break;

                        case commandsDefinitions.TableCommands.Destroy:
                            if (args.Length < 2)
                            {
                                throw new ArgumentException("Table destruction requires table name.");
                            }
                            query = $"DROP TABLE {args[1]}";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                comm.ExecuteNonQuery();
                            }
                            break;

                        case commandsDefinitions.TableCommands.AddValue:
                            if (args.Length < 4)
                            {
                                throw new ArgumentException("Adding a value requires table name, column, and value.");
                            }
                            query = $"INSERT INTO {args[1]} ({args[2]}) VALUES ({args[3]})";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                comm.ExecuteNonQuery();
                            }
                            break;

                        case commandsDefinitions.TableCommands.GetValue:
                            if (args.Length < 4)
                            {
                                throw new ArgumentException("Getting a value requires column, table name, and ID.");
                            }
                            query = $"SELECT {args[1]} FROM {args[2]} WHERE Id = @Id";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                comm.Parameters.AddWithValue("@Id", args[3]);
                                using (SqliteDataReader datareader = comm.ExecuteReader())
                                {
                                    while (datareader.Read())
                                    {
                                        GetValueResult = datareader[args[1]].ToString();
                                    }
                                }
                            }
                            break;

                        case commandsDefinitions.TableCommands.View:
                            if (args.Length < 2)
                            {
                                throw new ArgumentException("Viewing a table requires table name.");
                            }
                            query = $"SELECT * FROM {args[1]}";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                using (SqliteDataReader datareader = comm.ExecuteReader())
                                {
                                    System.Console.WriteLine("Habits");
                                    System.Console.WriteLine("------------");

                                    while (datareader.Read())
                                    {
                                        for (int i = 0; i < datareader.FieldCount; i++)
                                        {
                                            System.Console.WriteLine($"{datareader.GetName(i)}: {datareader.GetValue(i)}");
                                        }
                                    }
                                    System.Console.WriteLine("Pressione qualquer tecla para continuar...");
                                    Console.ReadKey();
                                }
                            }
                            break;

                        default:
                            ShowHelp(TableCommandHelp);
                            break;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error: " + e);
                }
            }
        }    
    }

}
