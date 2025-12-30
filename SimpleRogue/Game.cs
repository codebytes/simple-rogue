namespace SimpleRogue;

public enum GameState
{
    Playing,
    GameOver,
    Victory
}

public class Game
{
    private readonly Dungeon _dungeon;
    private readonly Player _player;
    private readonly List<Enemy> _enemies;
    private readonly List<Item> _items;
    private readonly List<string> _messageLog;
    public GameState State { get; private set; }

    public Game()
    {
        _dungeon = new Dungeon(80, 24);
        _player = new Player(_dungeon.GetRandomFloorPosition());
        _enemies = new List<Enemy>();
        _items = new List<Item>();
        _messageLog = new List<string>();
        State = GameState.Playing;

        SpawnEnemies();
        SpawnItems();
        AddMessage("Welcome to Simple Rogue! Defeat all enemies to win. Press 'q' to quit.");
    }

    public void Render(Action<string> displayCallback)
    {
        var buffer = new char[_dungeon.Width, _dungeon.Height];

        // Draw dungeon
        for (int x = 0; x < _dungeon.Width; x++)
        {
            for (int y = 0; y < _dungeon.Height; y++)
            {
                buffer[x, y] = _dungeon.GetTile(new Position(x, y)).GetSymbol();
            }
        }

        // Draw items
        foreach (var item in _items)
        {
            if (_dungeon.IsInBounds(item.Position.X, item.Position.Y))
            {
                buffer[item.Position.X, item.Position.Y] = item.Symbol;
            }
        }

        // Draw enemies
        foreach (var enemy in _enemies.Where(e => e.IsAlive))
        {
            if (_dungeon.IsInBounds(enemy.Position.X, enemy.Position.Y))
            {
                buffer[enemy.Position.X, enemy.Position.Y] = enemy.Symbol;
            }
        }

        // Draw player
        if (_dungeon.IsInBounds(_player.Position.X, _player.Position.Y))
        {
            buffer[_player.Position.X, _player.Position.Y] = _player.Symbol;
        }

        // Build output
        var output = new System.Text.StringBuilder();
        for (int y = 0; y < _dungeon.Height; y++)
        {
            for (int x = 0; x < _dungeon.Width; x++)
            {
                output.Append(buffer[x, y]);
            }
            output.AppendLine();
        }

        displayCallback(output.ToString());
    }

    public void ProcessInput(ConsoleKeyInfo key)
    {
        if (State != GameState.Playing)
        {
            return;
        }

        Position? newPosition = null;

        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.W or ConsoleKey.K:
                newPosition = _player.Position.Move(0, -1);
                break;
            case ConsoleKey.DownArrow or ConsoleKey.S or ConsoleKey.J:
                newPosition = _player.Position.Move(0, 1);
                break;
            case ConsoleKey.LeftArrow or ConsoleKey.A or ConsoleKey.H:
                newPosition = _player.Position.Move(-1, 0);
                break;
            case ConsoleKey.RightArrow or ConsoleKey.D or ConsoleKey.L:
                newPosition = _player.Position.Move(1, 0);
                break;
        }

        if (newPosition != null)
        {
            TryMovePlayer(newPosition);
            EnemyTurn();
            CheckGameState();
        }
    }

    private void TryMovePlayer(Position newPosition)
    {
        // Check for enemy at new position
        var enemy = _enemies.FirstOrDefault(e => e.IsAlive && e.Position == newPosition);
        if (enemy != null)
        {
            AttackEnemy(enemy);
            return;
        }

        // Check if position is walkable
        if (!_dungeon.IsWalkable(newPosition))
        {
            AddMessage("You can't move there!");
            return;
        }

        // Move player
        _player.Position = newPosition;

        // Check for items
        var item = _items.FirstOrDefault(i => i.Position == newPosition);
        if (item != null)
        {
            item.Use(_player);
            _items.Remove(item);

            if (item is HealthPotion)
            {
                AddMessage($"You picked up a {item.Name} and restored health!");
            }
            else if (item is Gold)
            {
                AddMessage($"You picked up {item.Name}!");
            }
        }
    }

    private void AttackEnemy(Enemy enemy)
    {
        int damage = _player.Attack;
        enemy.TakeDamage(damage);
        AddMessage($"You hit the {enemy.Name} for {damage} damage!");

        if (!enemy.IsAlive)
        {
            AddMessage($"The {enemy.Name} has been defeated!");
        }
    }

    private void EnemyTurn()
    {
        foreach (var enemy in _enemies.Where(e => e.IsAlive))
        {
            // Simple AI: move towards player if adjacent, otherwise random movement
            int dx = Math.Sign(_player.Position.X - enemy.Position.X);
            int dy = Math.Sign(_player.Position.Y - enemy.Position.Y);

            // Check if adjacent to player
            if (Math.Abs(_player.Position.X - enemy.Position.X) <= 1 &&
                Math.Abs(_player.Position.Y - enemy.Position.Y) <= 1 &&
                (dx != 0 || dy != 0))
            {
                // Attack player
                int damage = enemy.Attack;
                _player.TakeDamage(damage);
                AddMessage($"The {enemy.Name} hits you for {damage} damage!");
            }
            else
            {
                // Try to move towards player
                Position newPosition;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    newPosition = enemy.Position.Move(dx, 0);
                }
                else if (dy != 0)
                {
                    newPosition = enemy.Position.Move(0, dy);
                }
                else
                {
                    continue;
                }

                // Only move if position is walkable and not occupied by another enemy
                if (_dungeon.IsWalkable(newPosition) &&
                    !_enemies.Any(e => e.IsAlive && e.Position == newPosition && e != enemy))
                {
                    enemy.Position = newPosition;
                }
            }
        }
    }

    private void CheckGameState()
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

    private void SpawnEnemies()
    {
        var random = new Random();
        int enemyCount = random.Next(5, 10);

        for (int i = 0; i < enemyCount; i++)
        {
            var position = GetRandomEmptyPosition();
            var enemyType = random.Next(3);

            Enemy enemy = enemyType switch
            {
                0 => Enemy.CreateGoblin(position),
                1 => Enemy.CreateOrc(position),
                _ => Enemy.CreateTroll(position)
            };

            _enemies.Add(enemy);
        }
    }

    private void SpawnItems()
    {
        var random = new Random();
        int potionCount = random.Next(3, 6);
        int goldCount = random.Next(5, 10);

        for (int i = 0; i < potionCount; i++)
        {
            var position = GetRandomEmptyPosition();
            _items.Add(new HealthPotion(position));
        }

        for (int i = 0; i < goldCount; i++)
        {
            var position = GetRandomEmptyPosition();
            _items.Add(new Gold(position, random.Next(5, 20)));
        }
    }

    private Position GetRandomEmptyPosition()
    {
        int attempts = 0;
        const int maxAttempts = 100;

        while (attempts < maxAttempts)
        {
            var position = _dungeon.GetRandomFloorPosition();

            if (position != _player.Position &&
                !_enemies.Any(e => e.Position == position) &&
                !_items.Any(i => i.Position == position))
            {
                return position;
            }

            attempts++;
        }

        return _dungeon.GetRandomFloorPosition();
    }

    private void AddMessage(string message)
    {
        _messageLog.Add(message);
        if (_messageLog.Count > 5)
        {
            _messageLog.RemoveAt(0);
        }
    }

    public string GetStatusBar()
    {
        return $"Health: {_player.Health}/{_player.MaxHealth} | Gold: {_player.Gold} | " +
               $"Enemies: {_enemies.Count(e => e.IsAlive)}/{_enemies.Count}";
    }

    public IEnumerable<string> GetMessages() => _messageLog;
}
