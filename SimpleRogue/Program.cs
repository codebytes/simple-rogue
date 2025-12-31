using System.Reflection;
using SimpleRogue;

// Handle --version command
if (args.Length > 0 && args[0] is "--version" or "-v")
{
    var infoVersion = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion 
        ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString() 
        ?? "unknown";
    Console.WriteLine($"SimpleRogue v{infoVersion}");
    return;
}

// Show title screen
ConsoleUI.Clear();
ConsoleUI.ShowTitle("Simple Rogue");
ConsoleUI.ShowSubtitle("A basic console dungeon crawler");
ConsoleUI.NewLine();

await UpdateManager.CheckForUpdatesAsync();

ConsoleUI.NewLine();
ConsoleUI.WaitForKeyPress("Press any key to start...");

// Main game loop
var game = new Game();

while (game.State == GameState.Playing)
{
    ConsoleUI.Clear();
    ConsoleUI.ShowGamePanel(game.RenderToString());
    ConsoleUI.ShowStatusBar(game.GetStatusBar());
    ConsoleUI.NewLine();
    ConsoleUI.ShowMessageLog(game.GetMessages());
    ConsoleUI.NewLine();
    ConsoleUI.ShowControls("Controls: Arrow keys or WASD/HJKL to move | Q to quit");

    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q) break;
    
    game.ProcessInput(key);
}

// Show end screen
if (game.State == GameState.GameOver) ConsoleUI.ShowGameOver();
else if (game.State == GameState.Victory) ConsoleUI.ShowVictory();

ConsoleUI.ShowExitMessage();
