using System.Runtime.InteropServices;

namespace SQLITEAPI.commandsDefinitions 
{
    // This is meant to be a class solely for documentation purposes, later I'll add the functionality of a help command
    public static class databaseCommands
    {
        public const string Create = "create";
        public static readonly string CreateDescription = "Creates a database with a specified name.";
        public const string Delete = "delete";
        public static readonly string DeleteDescription = "Deletes a database with a specified name.";
        public const string Select = "select";
        public static readonly string SelectDescription = "Creates a file that specifies the selected database for future Table commands, if one already exists, overwrite it.";

        
    }

    public static class TableCommands
    {
        public const string Create = "create";
        public static readonly string CreateDescription = "Creates a table linked to the selected database.";
        public const string AddValue = "addvalue";
        public static readonly string AddValueDescription = "Adds a value to the specified table, in the specified columns in the selected database.";
        public const string GetValue = "getvalue";
        public static readonly string GetValueDescription = "Gets a single value specified by an ID and a column(s) in the selected database.";
        public const string Destroy = "destroy";
        public static readonly string DestroyDescription = "Destroys the specified table in the selected database.";
        public const string View = "view";
        public static readonly string ViewDescription = "Views all the values of the specified table of the selected database.";
    }
}