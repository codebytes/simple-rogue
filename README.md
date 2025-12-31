# Simple Rogue

A basic console rogue-like game implemented in .NET 10, inspired by classic dungeon crawlers like Rogue and NetHack.

## Features

- **Procedurally generated dungeons** with rooms and corridors
- **Player character (@)** with health and combat abilities
- **Multiple enemy types**: Goblins (g), Orcs (O), and Trolls (T)
- **Items**: Health potions (!) and gold ($)
- **Turn-based combat** system
- **Enhanced console UI** using Spectre.Console library
- **ASCII-based graphics** for retro feel
- **Automatic updates** via GitHub Releases using Updatum

## Requirements

- .NET 10.0 SDK or later (for building from source)
- Pre-built binaries available for Windows, Linux, and macOS in [Releases](https://github.com/codebytes/simple-rogue/releases)

## Installation

### Option 1: Download Pre-built Binary
1. Go to the [Releases](https://github.com/codebytes/simple-rogue/releases) page
2. Download the appropriate file for your operating system:
   - **Windows**: `SimpleRogue_win-x64_v*.exe`
   - **Linux**: `SimpleRogue_linux-x64_v*.zip`
   - **macOS (Intel)**: `SimpleRogue_osx-x64_v*.zip`
   - **macOS (Apple Silicon)**: `SimpleRogue_osx-arm64_v*.zip`
3. Extract (if zip) and run the executable

### Option 2: Build from Source

```bash
cd SimpleRogue
dotnet build
```

## How to Run

### From Binary
Just run the downloaded executable. The game will automatically check for updates on startup.

### From Source
```bash
cd SimpleRogue
dotnet run
```

## Controls

- **Arrow Keys** or **WASD** or **HJKL** (vi-style): Move in four directions
- **Q**: Quit game

## Gameplay

- Navigate through the dungeon using the movement keys
- Walk into enemies to attack them
- Pick up health potions (!) to restore health
- Collect gold ($) to increase your score
- Defeat all enemies to win the game
- Don't let your health reach zero!

## Game Elements

| Symbol | Description |
|--------|-------------|
| @      | Player      |
| g      | Goblin (30 HP, 5 ATK) |
| O      | Orc (50 HP, 8 ATK) |
| T      | Troll (80 HP, 12 ATK) |
| !      | Health Potion (restores 30 HP) |
| $      | Gold |
| .      | Floor (walkable) |
| #      | Wall |
| +      | Door |

## Technologies Used

- **.NET 10.0**: Latest .NET framework
- **Spectre.Console**: Enhanced console UI library for beautiful terminal output
- **Updatum**: Automatic application updates via GitHub Releases
- **C# 13**: Modern C# features including records, pattern matching, and top-level statements
- **GitVersion**: Semantic versioning for automated releases

## Continuous Integration

This project uses GitHub Actions for automated builds and releases:
- **Semantic Versioning**: Automatically determines version numbers using GitVersion
- **Multi-platform Builds**: Builds for Windows (x64), Linux (x64), macOS (x64 and ARM64)
- **Automated Releases**: Creates GitHub releases with properly named artifacts compatible with Updatum
- **Auto-update Support**: Built-in update checking and installation

## Project Structure

```
SimpleRogue/
├── Program.cs        - Main game loop and UI rendering
├── Game.cs           - Core game logic and state management
├── Entity.cs         - Player and enemy classes
├── Item.cs           - Health potions and gold
├── Dungeon.cs        - Procedural dungeon generation
├── Position.cs       - Position record for 2D coordinates
└── UpdateManager.cs  - Automatic update management
```

## Releasing

The project uses GitVersion for semantic versioning. To create a new release:

1. Merge changes to the `main` branch
2. Create and push a version tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. GitHub Actions will automatically:
   - Build the project for all platforms
   - Create a GitHub release with the tag
   - Upload all platform-specific binaries
   - Generate release notes

The application will automatically detect and offer to install these updates.

## License

This is a basic educational project demonstrating rogue-like game concepts in .NET.

