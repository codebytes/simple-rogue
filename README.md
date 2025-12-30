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

## Requirements

- .NET 10.0 SDK or later

## How to Build

```bash
cd SimpleRogue
dotnet build
```

## How to Run

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
- **C# 13**: Modern C# features including records, pattern matching, and top-level statements

## Project Structure

```
SimpleRogue/
├── Program.cs       - Main game loop and UI rendering
├── Game.cs          - Core game logic and state management
├── Entity.cs        - Player and enemy classes
├── Item.cs          - Health potions and gold
├── Dungeon.cs       - Procedural dungeon generation
└── Position.cs      - Position record for 2D coordinates
```

## License

This is a basic educational project demonstrating rogue-like game concepts in .NET.

