using System.Collections;
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
            File.OpenWrite($"{args[1]}.db");
            break;
            case commandsDefinitions.databaseCommands.Delete:
            CheckIfDatabaseExists($"{args[1]}.db");
            File.Delete($"{args[1]}.db");
            break;
            case commandsDefinitions.databaseCommands.Select:
            File.WriteAllText("selectedDatabase.txt", args[1]);
            connectionString = $"Datasource={File.ReadAllText("selectedDatabase.txt")}.db;Mode=ReadWriteCreate;Cache=Shared";

            break;
            default:
            ShowHelp(DatabaseCommandHelp);
            break;
            
            }
        }





        public static void TableCommand(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ShowHelp(TableCommandHelp);
                return;
            }

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try 
                {
                    conn.Open();
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
                            query = $"INSERT INTO {args[1]} ({args[2]}) VALUES (@Value)";
                            using (SqliteCommand comm = new SqliteCommand(query, conn))
                            {
                                comm.Parameters.AddWithValue("@Value", args[3]);
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
                                    Console.ReadKey();
                                }
                            }
                            break;

                        default:
                            ShowHelp(TableCommandHelp);
                            break;
                    }

                }
                finally
                {
                    conn.Close();
                }
                
                
            }
        }    
    }

}
