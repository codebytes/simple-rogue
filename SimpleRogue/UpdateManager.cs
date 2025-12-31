using Spectre.Console;
using Updatum;
using System.ComponentModel;

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
            
            // Subscribe to state changes
            updater.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UpdatumManager.State))
                {
                    AnsiConsole.MarkupLine($"[grey]Update status: {updater.State}[/]");
                }
                else if (e.PropertyName == nameof(UpdatumManager.DownloadedPercentage))
                {
                    AnsiConsole.MarkupLine($"[yellow]Downloaded: {updater.DownloadedPercentage}%[/]");
                }
            };
            
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
            
            AnsiConsole.MarkupLine($"[green]Update available: {release.TagName}[/]");
            AnsiConsole.MarkupLine($"[grey]{release.Name}[/]");
            
            if (AnsiConsole.Confirm("Would you like to download and install the update?"))
            {
                AnsiConsole.MarkupLine("[yellow]Downloading update...[/]");
                
                var download = await updater.DownloadUpdateAsync(release);
                if (download is null)
                {
                    AnsiConsole.MarkupLine("[red]Download failed.[/]");
                    return;
                }
                
                AnsiConsole.MarkupLine("[green]Download complete![/]");
                AnsiConsole.MarkupLine("[yellow]Installing update...[/]");
                
                // Install the update (this will download and replace the current executable)
                await updater.InstallUpdateAsync(download);
                
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
