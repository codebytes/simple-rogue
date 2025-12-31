using System.Text;

namespace SimpleRogue;

public enum GameState { Playing, GameOver, Victory }

public class Game
{
    private readonly Dungeon _dungeon;
    private readonly Player _player;
    private readonly List<Enemy> _enemies = [];
    private readonly List<Item> _items = [];
    private readonly List<string> _messageLog = [];
    private readonly Random _random = new();

    public GameState State { get; private set; } = GameState.Playing;

    // Input key to direction mapping
    private static readonly Dictionary<ConsoleKey, (int dx, int dy)> MovementKeys = new()
    {
        [ConsoleKey.UpArrow] = (0, -1),    [ConsoleKey.W] = (0, -1), [ConsoleKey.K] = (0, -1),
        [ConsoleKey.DownArrow] = (0, 1),   [ConsoleKey.S] = (0, 1),  [ConsoleKey.J] = (0, 1),
        [ConsoleKey.LeftArrow] = (-1, 0),  [ConsoleKey.A] = (-1, 0), [ConsoleKey.H] = (-1, 0),
        [ConsoleKey.RightArrow] = (1, 0),  [ConsoleKey.D] = (1, 0),  [ConsoleKey.L] = (1, 0)
    };

    public Game()
    {
        _dungeon = new Dungeon(80, 24);
        _player = new Player(_dungeon.GetRandomFloorPosition());

        SpawnEntities();
        AddMessage("Welcome to Simple Rogue! Defeat all enemies to win. Press 'q' to quit.");
    }

    public string RenderToString()
    {
        var buffer = new char[_dungeon.Width, _dungeon.Height];

        // Layer 1: Dungeon tiles
        for (int y = 0; y < _dungeon.Height; y++)
            for (int x = 0; x < _dungeon.Width; x++)
                buffer[x, y] = _dungeon.GetTile(new Position(x, y)).GetSymbol();

        // Layer 2: Items
        foreach (var item in _items)
            if (_dungeon.IsInBounds(item.Position.X, item.Position.Y))
                buffer[item.Position.X, item.Position.Y] = item.Symbol;

        // Layer 3: Enemies (alive only)
        foreach (var enemy in _enemies.Where(e => e.IsAlive))
            if (_dungeon.IsInBounds(enemy.Position.X, enemy.Position.Y))
                buffer[enemy.Position.X, enemy.Position.Y] = enemy.Symbol;

        // Layer 4: Player (top layer)
        if (_dungeon.IsInBounds(_player.Position.X, _player.Position.Y))
            buffer[_player.Position.X, _player.Position.Y] = _player.Symbol;

        // Convert to string
        var output = new StringBuilder();
        for (int y = 0; y < _dungeon.Height; y++)
        {
            for (int x = 0; x < _dungeon.Width; x++)
                output.Append(buffer[x, y]);
            output.AppendLine();
        }
        return output.ToString();
    }

    public void ProcessInput(ConsoleKeyInfo key)
    {
        if (State != GameState.Playing) return;
        if (!MovementKeys.TryGetValue(key.Key, out var direction)) return;

        var newPosition = _player.Position.Move(direction.dx, direction.dy);
        TryMovePlayer(newPosition);
        ProcessEnemyTurns();
        UpdateGameState();
    }

    private void TryMovePlayer(Position newPosition)
    {
        // Attack enemy if present
        var enemy = _enemies.FirstOrDefault(e => e.IsAlive && e.Position == newPosition);
        if (enemy != null)
        {
            Attack(_player, enemy);
            if (!enemy.IsAlive) AddMessage($"The {enemy.Name} has been defeated!");
            return;
        }

        // Check walkability
        if (!_dungeon.IsWalkable(newPosition))
        {
            AddMessage("You can't move there!");
            return;
        }

        // Move and collect items
        _player.Position = newPosition;
        var item = _items.FirstOrDefault(i => i.Position == newPosition);
        if (item != null)
        {
            item.Apply(_player);
            _items.Remove(item);
            AddMessage(item.GetPickupMessage());
        }
    }

    private void Attack(Entity attacker, Entity target)
    {
        target.TakeDamage(attacker.Attack);
        AddMessage($"{(attacker == _player ? "You hit" : $"The {attacker.Name} hits you for")} " +
                   $"{(attacker == _player ? $"the {target.Name} for " : "")}{attacker.Attack} damage!");
    }

    private void ProcessEnemyTurns()
    {
        foreach (var enemy in _enemies.Where(e => e.IsAlive))
        {
            var dx = Math.Sign(_player.Position.X - enemy.Position.X);
            var dy = Math.Sign(_player.Position.Y - enemy.Position.Y);
            var distX = Math.Abs(_player.Position.X - enemy.Position.X);
            var distY = Math.Abs(_player.Position.Y - enemy.Position.Y);

            // Adjacent to player? Attack!
            if (distX <= 1 && distY <= 1 && (dx != 0 || dy != 0))
            {
                _player.TakeDamage(enemy.Attack);
                AddMessage($"The {enemy.Name} hits you for {enemy.Attack} damage!");
                continue;
            }

            // Move towards player
            var newPos = distX > distY ? enemy.Position.Move(dx, 0) 
                       : dy != 0 ? enemy.Position.Move(0, dy) 
                       : enemy.Position;

            if (_dungeon.IsWalkable(newPos) && !_enemies.Any(e => e.IsAlive && e.Position == newPos && e != enemy))
                enemy.Position = newPos;
        }
    }

    private void UpdateGameState()
    {
        if (!_player.IsAlive)
        {
            State = GameState.GameOver;
            AddMessage("You have died! Game Over.");
        }
        else if (_enemies.All(e => !e.IsAlive))
        {
            State = GameState.Victory;
            AddMessage($"Victory! You defeated all enemies! Gold collected: {_player.Gold}");
        }
    }

    private void SpawnEntities()
    {
        // Spawn enemies
        int enemyCount = _random.Next(5, 10);
        for (int i = 0; i < enemyCount; i++)
            _enemies.Add(Enemy.CreateRandom(GetRandomEmptyPosition(), _random));

        // Spawn potions
        int potionCount = _random.Next(3, 6);
        for (int i = 0; i < potionCount; i++)
            _items.Add(new HealthPotion(GetRandomEmptyPosition()));

        // Spawn gold
        int goldCount = _random.Next(5, 10);
        for (int i = 0; i < goldCount; i++)
            _items.Add(new Gold(GetRandomEmptyPosition(), _random.Next(5, 20)));
    }

    private Position GetRandomEmptyPosition()
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            var pos = _dungeon.GetRandomFloorPosition();
            if (pos != _player.Position && 
                !_enemies.Any(e => e.Position == pos) && 
                !_items.Any(i => i.Position == pos))
                return pos;
        }
        return _dungeon.GetRandomFloorPosition();
    }

    private void AddMessage(string message)
    {
        _messageLog.Add(message);
        if (_messageLog.Count > 5) _messageLog.RemoveAt(0);
    }

    public string GetStatusBar() =>
        $"Health: {_player.Health}/{_player.MaxHealth} | Gold: {_player.Gold} | " +
        $"Enemies: {_enemies.Count(e => e.IsAlive)}/{_enemies.Count}";

    public IEnumerable<string> GetMessages() => _messageLog;
}
