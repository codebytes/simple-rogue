using Spectre.Console;
using Updatum;
using System.Reflection;

namespace SimpleRogue;

/// <summary>
/// Manages application updates using Updatum library
/// </summary>
public static class UpdateManager
{
    private const string RepositoryOwner = "codebytes";
    private const string RepositoryName = "simple-rogue";
    
    /// <summary>
    /// Check for updates and optionally install them
    /// </summary>
    public static async Task CheckForUpdatesAsync()
    {
        try
        {
            AnsiConsole.MarkupLine("[grey]Checking for updates...[/]");
            
            var updater = new UpdatumManager(RepositoryOwner, RepositoryName);
            
            // Check for updates
            var updateFound = await updater.CheckForUpdatesAsync();
            
            if (!updateFound)
            {
                AnsiConsole.MarkupLine("[grey]You are running the latest version.[/]");
                return;
            }
            
            var release = updater.LatestRelease;
            if (release is null)
            {
                AnsiConsole.MarkupLine("[grey]No release information found.[/]");
                return;
            }
            
            var asset = updater.GetCompatibleReleaseAsset(release);
            if (asset is null)
            {
                AnsiConsole.MarkupLine("[grey]No compatible update found for your platform.[/]");
                return;
            }
            
            var currentVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            
            // Strip metadata (e.g., +commitsha) for cleaner display
            var displayVersion = currentVersion.Split('+')[0];
            
            AnsiConsole.MarkupLine($"[grey]Current version: v{displayVersion}[/]");
            AnsiConsole.MarkupLine($"[green]Update available: {release.TagName}[/]");
            
            if (AnsiConsole.Confirm("Would you like to download and install the update?"))
            {
                UpdatumDownloadedAsset? download = null;
                
                await AnsiConsole.Progress()
                    .AutoClear(false)
                    .Columns(
                    [
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new RemainingTimeColumn(),
                        new SpinnerColumn()
                    ])
                    .StartAsync(async ctx =>
                    {
                        var downloadTask = ctx.AddTask("[yellow]Downloading update[/]", maxValue: 100);
                        
                        // Subscribe to download progress
                        updater.PropertyChanged += (sender, e) =>
                        {
                            if (e.PropertyName == nameof(UpdatumManager.DownloadedPercentage))
                            {
                                downloadTask.Value = updater.DownloadedPercentage;
                            }
                        };
                        
                        download = await updater.DownloadUpdateAsync(release);
                        downloadTask.Value = 100;
                    });
                
                if (download is null)
                {
                    AnsiConsole.MarkupLine("[red]Download failed.[/]");
                    return;
                }
                
                AnsiConsole.MarkupLine("[green]Download complete![/]");
                
                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("[yellow]Installing update...[/]", async ctx =>
                    {
                        await updater.InstallUpdateAsync(download);
                    });
                
                AnsiConsole.MarkupLine("[green]Update installed! Please restart the application.[/]");
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            // Silently fail if update check fails (e.g., no internet connection)
            AnsiConsole.MarkupLine($"[grey]Could not check for updates: {ex.Message}[/]");
        }
    }
}
