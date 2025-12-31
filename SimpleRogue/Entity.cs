namespace SimpleRogue;

public abstract class Entity
{
    public Position Position { get; set; }
    public char Symbol { get; init; }
    public int Health { get; set; }
    public int MaxHealth { get; init; }
    public int Attack { get; init; }
    public string Name { get; init; }

    protected Entity(Position position, char symbol, int maxHealth, int attack, string name)
    {
        Position = position;
        Symbol = symbol;
        MaxHealth = maxHealth;
        Health = maxHealth;
        Attack = attack;
        Name = name;
    }

    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
}

public class Player : Entity
{
    public int Gold { get; set; }

    public Player(Position position) 
        : base(position, '@', 100, 10, "Player")
    {
        Gold = 0;
    }
}

public class Enemy : Entity
{
    public Enemy(Position position, char symbol, int maxHealth, int attack, string name)
        : base(position, symbol, maxHealth, attack, name)
    {
    }

    public static Enemy CreateGoblin(Position position) 
        => new(position, 'g', 30, 5, "Goblin");

    public static Enemy CreateOrc(Position position) 
        => new(position, 'O', 50, 8, "Orc");

    public static Enemy CreateTroll(Position position) 
        => new(position, 'T', 80, 12, "Troll");
}
