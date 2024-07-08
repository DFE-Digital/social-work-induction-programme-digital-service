global using System.CommandLine;
global using Microsoft.Extensions.Configuration;
using Dfe.Sww.Ecf.Cli;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets(typeof(Program).Assembly)
    .Build();

var rootCommand = new RootCommand("Development tools for the Social Worker Workforce - Early Careers Framework.")
{
    Commands.CreateMigrateDbCommand(configuration),
    Commands.CreateCreateAdminCommand(configuration),
    Commands.CreateGenerateKeyCommand(configuration),
};

return await rootCommand.InvokeAsync(args);

public partial class Program { }
