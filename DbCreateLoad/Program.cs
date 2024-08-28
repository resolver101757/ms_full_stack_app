using DbUp;
using System;
using System.Reflection;

class Program
{
    static int Main(string[] args)
    {
        // Initial connection to master database to create UserDB
        var initialConnectionString = "Server=db; Database=master; User Id=sa; Password=YourStrong@Passw0rd;Encrypt=True;TrustServerCertificate=True;";

        // Configure DbUp for initial connection to create the database
        var initialUpgrader = DeployChanges.To
            .SqlDatabase(initialConnectionString)
            .WithScript("000_CreateDatabase", @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UserDB')
                BEGIN
                    CREATE DATABASE UserDB;
                END")
            .LogToConsole()
            .Build();

        var initialResult = initialUpgrader.PerformUpgrade();

        if (!initialResult.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(initialResult.Error);
            Console.ResetColor();
            return -1;
        }

        // Now connect to the newly created (or existing) UserDB to apply migrations
        var connectionString = "Server=db; Database=UserDB; User Id=sa; Password=YourStrong@Passw0rd;Encrypt=True;TrustServerCertificate=True;";

        // Configure DbUp for migrations in UserDB
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}
