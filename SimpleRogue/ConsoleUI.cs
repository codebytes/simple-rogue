using Spectre.Console;

namespace SimpleRogue;

/// <summary>
/// Centralized console UI rendering using Spectre.Console.
/// Provides reusable methods for all ANSI/styled console output.
/// </summary>
public static class ConsoleUI
{
    // Color constants for consistent theming
    public static readonly Color PrimaryColor = Color.Green;
    public static readonly Color SecondaryColor = Color.Yellow;
    public static readonly Color DangerColor = Color.Red;
    public static readonly Color MutedColor = Color.Grey;
    public static readonly Color HighlightColor = Color.Cyan1;

    /// <summary>
    /// Clears the console screen.
    /// </summary>
    public static void Clear() => AnsiConsole.Clear();

    /// <summary>
    /// Displays the game title using FigletText.
    /// </summary>
    /// <param name="title">The title text to display.</param>
    /// <param name="color">Optional color override.</param>
    public static void ShowTitle(string title, Color? color = null)
    {
        AnsiConsole.Write(new FigletText(title).Color(color ?? PrimaryColor));
    }

    /// <summary>
    /// Displays a subtitle/tagline below the title.
    /// </summary>
    /// <param name="text">The subtitle text.</param>
    public static void ShowSubtitle(string text)
    {
        AnsiConsole.MarkupLine($"[{SecondaryColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a muted/grey message.
    /// </summary>
    /// <param name="text">The message text.</param>
    public static void ShowMuted(string text)
    {
        AnsiConsole.MarkupLine($"[{MutedColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a success message in green.
    /// </summary>
    /// <param name="text">The message text.</param>
    public static void ShowSuccess(string text)
    {
        AnsiConsole.MarkupLine($"[bold {PrimaryColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays an error/danger message in red.
    /// </summary>
    /// <param name="text">The message text.</param>
    public static void ShowError(string text)
    {
        AnsiConsole.MarkupLine($"[bold {DangerColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a warning message in yellow.
    /// </summary>
    /// <param name="text">The message text.</param>
    public static void ShowWarning(string text)
    {
        AnsiConsole.MarkupLine($"[bold {SecondaryColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a highlighted/info message in cyan.
    /// </summary>
    /// <param name="text">The message text.</param>
    public static void ShowInfo(string text)
    {
        AnsiConsole.MarkupLine($"[{HighlightColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays bold text.
    /// </summary>
    /// <param name="text">The text to display.</param>
    public static void ShowBold(string text)
    {
        AnsiConsole.MarkupLine($"[bold]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a section header.
    /// </summary>
    /// <param name="text">The header text.</param>
    /// <param name="color">Optional color override.</param>
    public static void ShowHeader(string text, Color? color = null)
    {
        AnsiConsole.MarkupLine($"[bold {color ?? SecondaryColor}]{text.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a blank line.
    /// </summary>
    public static void NewLine() => AnsiConsole.WriteLine();

    /// <summary>
    /// Displays content inside a styled panel.
    /// </summary>
    /// <param name="content">The content to display (can include Spectre markup).</param>
    /// <param name="header">Optional panel header.</param>
    /// <param name="borderColor">Optional border color.</param>
    /// <param name="border">Optional border style.</param>
    public static void ShowPanel(string content, string? header = null, Color? borderColor = null, BoxBorder? border = null)
    {
        var panel = new Panel(new Markup(content))
        {
            Border = border ?? BoxBorder.Double,
            BorderStyle = new Style(borderColor ?? PrimaryColor)
        };

        if (header != null)
        {
            panel.Header = new PanelHeader($"[bold {borderColor ?? PrimaryColor}]{header.EscapeMarkup()}[/]");
        }

        AnsiConsole.Write(panel);
    }

    /// <summary>
    /// Displays the game map inside a styled panel.
    /// </summary>
    /// <param name="gameContent">The rendered game map content.</param>
    /// <param name="title">The game title for the panel header.</param>
    public static void ShowGamePanel(string gameContent, string title = "Simple Rogue")
    {
        var gamePanel = new Panel(new Markup(gameContent.EscapeMarkup()))
        {
            Header = new PanelHeader($"[bold {PrimaryColor}]{title.EscapeMarkup()}[/]"),
            Border = BoxBorder.Double,
            BorderStyle = new Style(PrimaryColor)
        };

        AnsiConsole.Write(gamePanel);
    }

    /// <summary>
    /// Displays the status bar with player stats.
    /// </summary>
    /// <param name="statusText">The status bar content.</param>
    public static void ShowStatusBar(string statusText)
    {
        AnsiConsole.MarkupLine($"[bold]{statusText.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a message log entry.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void ShowLogMessage(string message)
    {
        AnsiConsole.MarkupLine($"[{MutedColor}]> {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays multiple message log entries with a header.
    /// </summary>
    /// <param name="messages">The messages to display.</param>
    /// <param name="header">Optional header for the message section.</param>
    public static void ShowMessageLog(IEnumerable<string> messages, string header = "Messages:")
    {
        ShowHeader(header);
        foreach (var message in messages)
        {
            ShowLogMessage(message);
        }
    }

    /// <summary>
    /// Displays control hints.
    /// </summary>
    /// <param name="controls">The control hint text.</param>
    public static void ShowControls(string controls)
    {
        AnsiConsole.MarkupLine($"[{MutedColor}]{controls.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Prompts the user to press any key to continue.
    /// </summary>
    /// <param name="message">Optional custom message.</param>
    public static void WaitForKeyPress(string message = "Press any key to continue...")
    {
        ShowMuted(message);
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays a game over screen.
    /// </summary>
    public static void ShowGameOver()
    {
        ShowError("GAME OVER!");
        WaitForKeyPress("Press any key to exit...");
    }

    /// <summary>
    /// Displays a victory screen.
    /// </summary>
    public static void ShowVictory()
    {
        ShowSuccess("VICTORY!");
        WaitForKeyPress("Press any key to exit...");
    }

    /// <summary>
    /// Displays a thank you message when exiting.
    /// </summary>
    /// <param name="message">Optional custom message.</param>
    public static void ShowExitMessage(string message = "Thanks for playing!")
    {
        AnsiConsole.MarkupLine($"[{PrimaryColor}]{message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a rule (horizontal line with optional title).
    /// </summary>
    /// <param name="title">Optional title in the rule.</param>
    /// <param name="color">Optional color.</param>
    public static void ShowRule(string? title = null, Color? color = null)
    {
        var rule = new Rule(title ?? string.Empty)
        {
            Style = new Style(color ?? MutedColor)
        };
        AnsiConsole.Write(rule);
    }

    /// <summary>
    /// Displays a table with key-value pairs (useful for stats).
    /// </summary>
    /// <param name="data">Dictionary of key-value pairs.</param>
    /// <param name="title">Optional table title.</param>
    public static void ShowTable(Dictionary<string, string> data, string? title = null)
    {
        var table = new Table()
            .BorderColor(PrimaryColor)
            .Border(TableBorder.Rounded);

        table.AddColumn(new TableColumn("Stat").Centered());
        table.AddColumn(new TableColumn("Value").Centered());

        foreach (var kvp in data)
        {
            table.AddRow(kvp.Key.EscapeMarkup(), kvp.Value.EscapeMarkup());
        }

        if (title != null)
        {
            table.Title = new TableTitle(title, new Style(SecondaryColor));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Creates a progress bar for async operations.
    /// </summary>
    /// <param name="action">The async action to execute with progress context.</param>
    public static async Task WithProgressAsync(Func<ProgressContext, Task> action)
    {
        await AnsiConsole.Progress()
            .AutoRefresh(true)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn()
            )
            .StartAsync(action);
    }

    /// <summary>
    /// Shows a spinner while executing an async operation.
    /// </summary>
    /// <param name="message">The message to display with the spinner.</param>
    /// <param name="action">The async action to execute.</param>
    public static async Task WithSpinnerAsync(string message, Func<Task> action)
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(PrimaryColor))
            .StartAsync(message, async _ => await action());
    }
}
