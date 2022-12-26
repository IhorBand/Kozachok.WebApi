﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using DbUp;
using Microsoft.Extensions.Configuration;

Console.WriteLine("<============================================================>");
Console.WriteLine("<                          DbUpdate                          >");
Console.WriteLine("<============================================================>");
Console.WriteLine("⠀⠀⠀⠀⣠⣶⡾⠏⠉⠙⠳⢦⡀⠀⠀⠀⢠⠞⠉⠙⠉⠙⠲⡀⠀");
Console.WriteLine("⠀⠀⠀⣴⠿⠏⠀⠀⠀⠀⠀⠀ ⢳⡀⠀⡏⠀⠀We   ⢷");
Console.WriteLine("⠀⠀⢠⣟⣋⡀⢀⣀⣀⡀⠀⣀⡀ ⣧⠀⢸⠀will   ⡇");
Console.WriteLine("⠀⠀⢸⣯⡭⠁⠸⣛⣟⠆⡴⣻⡲ ⣿⠀⣸⠀punch  ⡇");
Console.WriteLine("⠀⠀⣟⣿⡭⠀⠀⠀⠀⠀⢱⠀⠀ ⣿⠀⢹⠀⠀this  ⡇");
Console.WriteLine("⠀⠀⠙⢿⣯⠄⠀⠀⠀⢀⡀⠀⠀⡿⠀⠀⡇⠀⠀⠀DB  ⡼");
Console.WriteLine("⠀⠀⠀⠀⠹⣶⠆⠀⠀⠀⠀⠀⡴⠃⠀⠀⠘⠤⣄⣠⣄⣠⣄⠞⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⢸⣷⡦⢤⡤⢤⣞⣁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⢀⣤⣴⣿⣏⠁⠀⠀⠸⣏⢯⣷⣖⣦⡀⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⢀⣾⣽⣿⣿⣿⣿⠛⢲⣶⣾⢉⡷⣿⣿⠵⣿⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⣼⣿⠍⠉⣿⡭⠉⠙⢺⣇⣼⡏⠀⠀⠀⣄⢸⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⣿⣿⣧⣀⣿.........⣀⣰⣏⣘⣆⣀⠀⠀");

var baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;

string? environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine($"Environment: {environmentVariable}");
var configuration = new ConfigurationBuilder()
        .SetBasePath(baseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environmentVariable}.json", optional: true, reloadOnChange: true)
        .Build();

var connectionString = configuration.GetConnectionString("Main");
string dbScriptFolder = string.Empty;

if (args != null && args.Length > 0)
{
    dbScriptFolder = Path.GetFullPath(args[0]);
}
else
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        dbScriptFolder = Path.GetFullPath(Path.Combine(baseDirectory == null ? string.Empty : baseDirectory, "..\\..\\..\\..\\database\\"));
    }
    else
    {
        dbScriptFolder = Path.GetFullPath(Path.Combine(baseDirectory == null ? string.Empty : baseDirectory, "../../../../database/"));
    }
}

Console.WriteLine($"db Script Folder: {dbScriptFolder}");

var upgrader =
    DeployChanges.To
        .SqlDatabase(connectionString)
        .WithScriptsFromFileSystem(dbScriptFolder)
        .WithTransaction()
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⣿⣶⣦⣤⣤⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣦⣤⣄⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠿⣿⠿⠿⠿⠿⡿⠿⠿⣿⡿⠇⠀⣀⢀⣀⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠈⠉⠙⠛⠻⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣶⣤⣤⣀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠒⠒⠦⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⠛⠛⠿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣦⣤⣤⣀⣀⣀⠀⠀⠀⠁⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠠⠤⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠙⠛⠛⠿⠿⣿⣿⣿⣿⣿⣿⣿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⡴⠶⠒⠚⠛⠛⠛⠛⠛⠛⠓⠲⠶⢤⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⠛⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣶⠟⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⠲⠦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠄⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣾⠋⠀⢀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠲⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠈⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣠⣤⣤⣤⣤⣤⣤⣤⣤⣠⣄⣀⣀⣄⡀⠀⠀⠀⠀⣰⡿⠁⠀⢰⣿⡿⠿⢿⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠲⣄⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣷⣶⣷⣿⠃⠀⢀⣾⣿⣤⣤⣤⣿⠟⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣦⡀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀⢠⣿⠋⠉⠛⢿⣏⠀⠀⢀⣤⣶⣿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣆⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣼⣿⣿⣿⡿⢋⡞⣹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⢡⣿⠇⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀⠘⠛⠷⢶⣤⣾⠿⢀⣴⡿⠛⠉⣿⡏⠀⠀⣼⣿⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢷⡀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠄⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⠋⣼⡟⠡⠏⠸⡿⠛⠛⠛⠛⠛⠛⣻⢛⡟⠃⢸⣿⢠⣿⣿⣿⣛⠛⢻⡛⡿⣿⡀⠀⠀⠀⠀⠀⠀⢠⣿⢿⣷⣤⣤⣿⠇⠀⣼⣿⣃⣠⣤⣶⡿⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣄⣀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠁⠀⠀⠀⠀⠀⠀⠄⠀⠀⠀⠀⠀⡨⠿⠛⠁⠈⠀⠀⠀⣀⣴⡠⠀⢠⢠⡎⠀⣰⡟⣼⢧⠀⣾⡏⢸⣿⣿⣿⣿⣿⣼⣧⣟⢿⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⢹⣿⢁⣼⡿⣿⣿⠛⠉⠀⠀⠀⠀⣀⣤⣶⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⣠⡔⠀⡿⠀⣿⣏⠄⢰⠏⡾⠀⣰⡿⠁⣿⣸⢠⣿⡇⢸⣿⣿⣿⣿⣿⣿⣿⣿⠈⢿⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠺⠏⠀⠘⣿⡀⠀⠀⠀⣠⣾⠿⢿⣿⠃⠀⠀⠀⠀⣠⣤⠀⠀⠀⠀⠸⣿");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⢀⠐⡁⠀⠀⠀⠀⠈⣀⣶⣶⠇⣾⣳⠁⣸⡃⣨⣿⠁⢠⣿⢸⡇⣰⣿⢡⡄⣿⣿⣸⣿⠇⣿⣿⣿⣿⣿⣿⡿⣿⡟⠀⠈⢻⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠿⠁⠀⣠⣾⣿⣁⠀⣸⣿⠀⠀⠀⢀⣼⠟⠁⠀⠀⠀⠀⠀⣿");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⢀⠀⠈⠀⢰⠀⠀⠀⢀⠀⣸⣿⡿⡿⣾⣿⠇⢰⣿⣰⣿⣇⠀⣾⣇⣿⣟⣿⣿⣾⣧⣿⣿⣿⣿⢠⣿⣿⣿⣿⣿⣿⡇⣿⡇⠀⠀⠀⢻⣿⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠛⠋⠛⠻⠿⣿⡧⠀⠀⣴⠟⠁⠀⠀⠀⠀⠀⠀⠀⣿");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⢀⠀⠁⠀⠀⠀⠈⠀⠀⠀⠀⣴⣿⣿⣿⣿⣿⣿⢀⣿⣿⣿⣿⣙⢷⣿⡿⢸⡾⣿⣿⣿⢻⣿⣿⣿⡏⣸⣿⣿⣿⣿⣿⣿⠃⣿⡇⠀⠀⠀⠀⠹⣿⣷⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠿⠃⣤⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿");
    Console.WriteLine("⠀⠀⡀⠔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠄⡰⢡⣿⣿⣿⣿⣿⡏⢸⣿⣿⣿⣿⢖⣿⣿⠁⢸⠇⣿⣿⣿⣾⢿⣿⣿⣷⣿⡏⣿⣿⣿⣿⡟⢠⣿⠀⠀⠀⠀⠀⠀⠹⣿⣿⣷⣦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⠏");
    Console.WriteLine("⠀⡜⠀⠀⠀⠀⠀⠀⢀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠈⢠⠁⣸⣿⣿⣿⣿⣿⡇⣼⣿⣧⣿⣿⣸⣿⠏⠀⢸⠀⣿⢻⣿⣿⢸⣿⣿⣿⣿⠃⢿⣿⣿⣿⣁⣼⡿⠤⠤⠤⣤⣀⣀⠀⠘⣿⣿⣿⣿⣷⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⡿⠋⠀");
    Console.WriteLine("⠈⠀⠀⠀⠀⠀⠀⡀⠁⠀⠀⠀⠀⠀⠀⠀⡄⠀⠐⢀⠆⠀⣿⡏⣿⣿⣿⣿⡇⣿⠿⢹⣿⣿⣿⡟⠀⣀⣸⡤⣿⢼⣿⣿⠀⣿⣿⣿⡿⠀⣸⣿⣿⠃⠀⡿⠀⠀⠀⠀⠀⠀⠈⠉⠑⠺⢿⣿⣿⣿⣿⣿⣿⣶⢤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣴⡿⠛⠁⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⢀⠈⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⠀⡄⠈⠀⢰⣿⢡⣿⣿⣿⣿⡇⡇⡿⣿⣿⣿⡿⠓⠉⠁⠈⡄⢻⠀⢿⣿⠀⢸⣿⣿⠃⢀⣿⡿⠃⢀⡼⠁⣀⣀⠤⠤⠀⠀⠀⠀⠀⠀⠀⠻⣿⣿⣿⣿⣿⣿⡇⠀⠉⠙⠒⢶⠤⣤⣤⣤⣤⣤⣤⣤⣤⣴⠶⠾⠛⠋⢹⡀⠀⠀⠀");
    Console.WriteLine("⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠞⠃⠀⡄⠀⠀⢸⣿⢸⣿⣿⣿⣿⣇⣏⠰⣿⣿⣿⠃⠀⠀⢀⡀⡤⠼⡤⠘⣿⡇⣾⣿⠏⠀⣾⠟⠁⢀⠊⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢿⣿⣿⣿⣿⣗⠀⠀⠀⠀⠠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣔⣁⣤⣾⠈⣰⠀⠀⢸⣿⢸⣿⣿⣿⣿⣿⡷⠀⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠃⠀⠘⣿⣿⡟⢀⠜⠁⠀⠀⠁⠀⠀⠀⠀⣀⣤⣤⣤⣤⣄⣀⣀⡀⠀⠀⠀⠈⠻⣿⣿⣿⣿⡀⠀⠀⠀⠀⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠦⣤⣤⣤");
    Console.WriteLine("⠁⠀⠀⠀⠀⠈⠀⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⢧⡈⠀⠀⢸⣿⢸⣿⣿⣿⣿⣿⡞⡂⣿⡿⡇⠀⣀⣠⣤⣶⣶⣶⣶⣥⠀⣸⠯⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠴⣿⣿⣿⣿⡟⣿⠟⠻⢿⣿⣤⡄⠀⠀⠈⣿⣿⣿⠃⠀⠀⠀⠀⠈⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⣠⣶⣾⡿⠟⠛⠉⠉⠀⢀⠤⠃⠀⠀⢸⣿⢸⣿⣿⣿⣻⣿⣿⣧⣿⠁⣷⣶⣿⣿⣿⣿⣯⡀⠀⢉⠑⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠛⠻⢻⣦⣿⣷⡄⢀⡠⠁⣹⡿⠦⠀⠀⣿⡟⠛⠀⠀⠀⠀⠀⠀⠈⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⢀⣠⡴⠟⠛⠉⠁⠀⠀⡠⠔⠂⠈⠀⠀⠀⠀⠀⠈⣿⣸⣿⣿⣿⣿⣿⣿⢿⣿⣠⣿⠃⠸⡟⢧⣾⣿⣿⠛⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⠈⠉⠫⠭⢥⠞⠁⠐⠉⠀⠀⠀⠀⣿⠇⢢⣇⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠴⠚⠉⠉⠁⠀⠀⣠⠤⠒⠀⠁⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡀⠸⡑⠀⠰⠤⠀⠁⠀⠈⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢏⠤⠤⢿⠄⠀⠀⠀⠀⠀⠀⠡⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠤⢴⡤⠄⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣿⣿⣿⣿⣿⣿⣿⡿⣿⡇⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠼⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠁⠐⢦⠃⠀⠀⠀⠀⠀⠀⠀⠀⠐⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠒⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⡇⢻⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡜⠀⢀⢞⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢢⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣧⡈⡈⢧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡜⠀⠀⣡⣾⠅⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⢠⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⣿⣿⠈⠂⠈⠳⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠀⣀⣴⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣤⡒⠉⠀⠀⠉⠒⠲⢄⣀⣄⡀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⣿⣿⠑⢄⡀⠀⠝⠂⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣀⣀⣀⣀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠸⠀⠀⣿⣿⣿⣿⣿⡿⠀⠀⠀⠀⠀⠀⠀⠀⢀⠁⠉⢫⠉⠙⠒⠂⠤⣀⡀⠀⠀⠀⠈⠁⠐");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠇⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⣿⠀⠈⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⣿⢻⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢆⠀⠀⠀⠀⠀⠀⠀⠀⠄⡀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⠀⠀⢹⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⣿⣿⣿⣿⣿⡿⠟⠁⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⣾⣿⣿⣿⡏⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⣿⠀⠀⠰⠁⠀⢠⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡁⠀⠀⢻⣿⣿⡿⠀⠀⠀⠀⠀⠀⠀⢠⠀⠁⠀⠀⠀⠀⠀⠀⠘⠂⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣤⡴⠃⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣿⠀⡀⠀⠀⢠⣾⣿⣷⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣾⣷⠀⠀⠘⣿⡟⠀⠀⠀⠀⠀⠀⠀⠠⠀⠀⠀⠀⠀⠀⣀⠐⠁⠀⠀⠀⠓⢀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⠈⠀⢀⠀⠀⡄⠀⠀⡆⠘⠻⣿⣿⠀⠀⠀⢠⠃⠈⠻⣿⣿⣿⣶⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣴⠟⢻⣿⣿⣆⠀⠀⢙⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⠀⠀⠀⠀⠀⠁⢀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣌⠄⠀⠀⠀⠈⠂⠈⠀⠀⢸⡇⠀⠀⢀⠀⠀⠘⠁⠀⣀⡴⠁⢠⠀⠀⠈⠛⢷⣬⡉⠙⠓⣦⣄⡀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣶⡿⠟⠁⣠⣿⣿⣿⣿⠆⠀⠀⠀⠷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠤⣼⡇⠀⠀⢸⠀⠀⢸⠄⠀⠀⠱⠀⠀⢃⠀⠀⠀⠀⠈⠙⠓⠲⠿⣿⣿⣷⣦⠤⠠⣤⣶⣿⣿⣿⠟⠁⠀⢠⣿⣿⣿⠟⠁⠀⠀⠀⠀⠀⠀⠁⡀⠀⠀⠀⠀⠀⠀⢀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⡀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⣠⠊⠀⠀⠀⠀⠀⢀⡼⠀⠀⠀⠀⠀⠀⠀⠄⠀⠀⠀⠀⠀⠀⠀⠠⣻⣿⣿⣿⠳⡄⠉⠉⠛⠁⠀⠀⠀⠀⣾⣿⣿⣿⡄⠀⠀⠀⠀⠀⠀⠠⠀⠈⠢⡀⠀⠀⠀⠀⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⠀⠀⢇⣀⠔⠀⣠⠤⡲⠟⠁⠀⠰⠅⡀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⠀⠘⢦⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⡀⠀⠀⠀⠀⡔⢇⠀⠀⠀⠈⠂⢄⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠀⠀");
    Console.WriteLine("⠀⠀⠀⢀⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⣰⠃⠘⠒⠏⠀⠘⠕⠀⠀⠀⠀⢀⡘⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣿⣿⡟⠀⠀⢸⡇⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣷⣀⠀⠀⠀⠁⠈⠁⠀⠀⠀⠀⠀⠁⠠⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⡠⢠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡶⠶⠀⠀⡰⠃⠀⠀⠀⠀⠀⠀⢀⣮⣤⡖⠘⠧⡱⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⣏⡀⠀⠀⠀⠀⠠⢴⣿⣆⠀⢀⠀⠀⠀⠀⠀⠀⠈⠢⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠀⠀");
    Console.WriteLine("⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⡿⢶⣚⡁⠀⠀⠀⠀⢀⢠⣾⣿⣿⣿⡇⢀⠀⠈⠙⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⡖⠀⠠⢀⣀⠈⠀⠘⡄⠈⠆⠀⠀⠀⠀⠀⠀⠀⠀⢻⣶⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⡀⠀⠀⠉⠁⠂⠀⠘⠁⣿⣿⣿⣿⣿⡇⠀⠁⡄⠀⠀⠱⠄⡀⠀⠀⠀⠀⠀⠀⠀⠀⣿⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⠁⠀⠀⣀⡠⠏⠲⠤⡀⠀⢸⢄⠀⠀⠀⠀⠀⠀⠀⠀⠻⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡆⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⡇⠀⠀⠈⠢⠀⠀⠀⠈⠢⠤⢄⡀⠀⠀⠀⢈⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⡇⢀⠀⠒⠁⠀⠀⠀⠀⠈⠑⠮⡩⡢⡀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢇⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠂⢀⡀⠀⠀⢻⣿⣿⣿⣿⣿⡇⣀⣀⣈⣢⣕⡀⠀⠀⠀⠀⠀⠈⠐⠒⠢⣼⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠈⠙⠚⠷⠦⡀⠀⢀⠈⠁⠀⠀⠀⠀⠀⠀⠀⠄⠀⠀⠀⠀⠀⠘⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⣿⣿⣿⣿⣷⠁⢰⣿⣿⣿⣧⡀⠀⠀⠀⠀⠀⠀⠀⣰⣿⠋⠉⠢⠤⠀⢀⠀⠀⠀⠀⢠⣿⣿⣿⣿⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠀⠀⠀⠀⠀⠀⠈⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
    Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡀⠀⠀⢀⠀⢸⣿⣿⣿⣿⣿⣿⣿⠀⠴⢾⣿⣿⣿⣿⣦⣄⠀⠀⠀⣠⣾⠟⠁⠀⠀⠀⠀⠀⠀⠁⠒⠀⡅⣼⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠘");

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.WriteLine("⠀                          ⢀⣠⣤⡤⠴⠶⠤⢤⣤⣀⡀⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠞⠋⠁⠀⠀⠀⠀⠀⠀⠀ ⠉⠙⠳⢦⣄⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠉⠛⢦⡀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠙⢦⡀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠈⢳⡄⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⢻⡄⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⢻⡄⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⢷⡀⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠸⣇⠀⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⣿⣿⠀⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⢸⣿⡇⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠀⠀⣀⣤⣤⣤⣄⣀⡀⠀⢀⣤⡄⠀⠀⣠⣄⠀⠀⢀⣀⠀⠀⠀⣤⡄⣠⡄⠀⢀⣀⡀⠀  ⡇⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡿⠀⠘⠿⣿⡟⠛⢫⣿⣇⣀⣼⣿⠁⠀⣴⣿⣿⠀⠀⣾⣿⣧⡀⢸⡿⢹⣿⣡⣶⡿⠛⠁⠀  ⡇⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⢰⣿⠃⠀⣼⡿⠿⢻⣿⠏⢀⣼⡟⣙⣿⡆⢰⣿⠻⣿⣷⣿⠇⣿⣿⣟⠁⠀⠀⠀⠀  ⡇⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⣾⡿⠀⢀⣿⡇⠀⢸⡿⢰⣿⠿⠿⠿⢿⡿⣾⡿⠀⠘⣿⡿⢸⣿⠿⢿⣷⡦⠀    ⡇");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⢀⠀⠀⠀⢀⡀⠀⠀⢀⣀⣀⡀⠀⠉⢀⠀⠀⠀⠀⠀⢀⣤⣤⠀⠀⠀⠀⠀  ⡇⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⣿⣇⢀⣴⣿⢟⣴⣾⠟⠛⢿⣿⣆⣰⣿⠃⠀⣼⣿⠁⣼⡿⠃⠀⠀⠀    ⡇ ");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠹⣿⣾⠟⠁⣾⣿⠁⠀⠀⢸⣿⣏⣿⡏⠀⢰⣿⠇⣼⡟⠁⠀⠀⠀⠀    ⡇⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⢀⣼⡿⠁⠀⠈⣿⣿⣄⣀⣴⣿⠏⢸⣿⣶⣾⣿⣿⣰⡿⠀⠀⠀⠀⠀     ⣿⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⠀⠀⠀⠀⠈⠋⠀⠀⠀⠀⠈⠙⠛⠛⠉⠀⠀⠈⠉⠉⠀⠙⠁⠉⠁⠀⠀⠀⠀⠀⠀    ⢿⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀  ⣼⠃⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀  ⣿⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀  ⢸⣿⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⢀⣿⣿⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⠀⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣿⠀⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡼⠋⠈⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⢦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⡶⠛⠀⠀⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠛⠶⠤⣄⣀⣀⣀⣀⣠⣴⠾⠛⠁⠀");
Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠁⠀           ");

Console.ResetColor();
return 0;