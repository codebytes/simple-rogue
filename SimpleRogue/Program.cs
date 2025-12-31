using SimpleRogue;
using Spectre.Console;

AnsiConsole.Clear();
AnsiConsole.Write(new FigletText("Simple Rogue").Color(Color.Green));
AnsiConsole.MarkupLine("[yellow]A basic console dungeon crawler[/]");
AnsiConsole.WriteLine();

// Check for updates
await UpdateManager.CheckForUpdatesAsync();

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine("[grey]Press any key to start...[/]");
Console.ReadKey(true);

var game = new Game();
bool running = true;

while (running)
{
    AnsiConsole.Clear();

    // Create a panel for the game
    var gamePanel = new Panel(
        new Markup(RenderGame(game))
    )
    {
        Header = new PanelHeader("[bold green]Simple Rogue[/]"),
        Border = BoxBorder.Double,
        BorderStyle = new Style(Color.Green)
    };

    AnsiConsole.Write(gamePanel);

    // Status bar
    AnsiConsole.MarkupLine($"[bold]{game.GetStatusBar()}[/]");
    AnsiConsole.WriteLine();

    // Messages
    AnsiConsole.MarkupLine("[bold yellow]Messages:[/]");
    foreach (var message in game.GetMessages())
    {
        AnsiConsole.MarkupLine($"[grey]> {message.EscapeMarkup()}[/]");
    }

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Controls: Arrow keys or WASD/HJKL to move | Q to quit[/]");

    // Check game state
    if (game.State == GameState.GameOver)
    {
        AnsiConsole.MarkupLine("[bold red]GAME OVER![/]");
        AnsiConsole.MarkupLine("[grey]Press any key to exit...[/]");
        Console.ReadKey(true);
        break;
    }
    else if (game.State == GameState.Victory)
    {
        AnsiConsole.MarkupLine("[bold green]VICTORY![/]");
        AnsiConsole.MarkupLine("[grey]Press any key to exit...[/]");
        Console.ReadKey(true);
        break;
    }

    // Get input
    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q)
    {
        running = false;
    }
    else
    {
        game.ProcessInput(key);
    }
}

AnsiConsole.MarkupLine("[green]Thanks for playing![/]");

static string RenderGame(Game game)
{
    var output = new System.Text.StringBuilder();
    game.Render(gameMap => output.Append(gameMap));
    return output.ToString().EscapeMarkup();
}
