using Spectre.Console;
using Spectre.Console.Cli;
using Tricorder.CLI;

var app = new CommandApp();

app.Configure(config =>
{
    config.Settings.ApplicationName = "Tricorder";
    config.AddCommand<QueryDirectoryCommand>("scan")
        .WithDescription("Scans a directory of HL7 messages and aggregates the results of a given HL7 path query.")
        .WithExample(new[] { "scan", "PV1.1.2" })
        .WithExample(new[] { "scan", "\"PID.4\"", "\"C:\\\\Data\\\\Incoming\"" });
});
try
{
    return await app.RunAsync(args);
}
catch(Exception exception)
{
    AnsiConsole.MarkupLine("[red]***Exception Encountered***[/]");
    AnsiConsole.WriteException(exception);
    return 1;
}