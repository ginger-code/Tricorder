using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Tricorder.CLI;

public class QueryDirectoryCommand : AsyncCommand<QueryDirectoryCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Pattern to query for. Must adhere to the pattern `(segment)(.fieldNumber)?(.componentNumber)?` where segment is a three letter code like `PID`, and fieldNumber/componentNumber are each a 1-based index. i.e. `PV1.1.2`, `PV1.2`, or `PV1`")]
        [CommandArgument(0, "<searchPattern>")]
        public string SearchPattern { get; init; } = null!;

        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(1, "[searchPath]")]
        public string SearchPath { get; init; } = Environment.CurrentDirectory;

    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var values = new List<string>();
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .StartAsync("Scanning...", async ctx => {
                values =  (await HL7.CollectValues(settings.SearchPattern, settings.SearchPath)).ToList();
            });
        if (values.Count == 0)
        {
            AnsiConsole.Markup("[red]Sorry, no results were found for that query.[/]");
            return 0;
        }
        var countDictionary = values.GroupBy(val => val).ToDictionary(grouping => grouping.Key, grouping => grouping.Count());
        AnsiConsole.MarkupLine("[green]Results:[/]");
        var table = new Table();
        table.AddColumn(new TableColumn("[green]Value[/]").Centered());
        table.AddColumn(new TableColumn("[blue]Count[/]").Centered());
        foreach (var key in countDictionary.Keys)
        {
            table.AddRow(key, $"{countDictionary[key]}");
        }
        AnsiConsole.Write(table);
        return 0;
    }

}