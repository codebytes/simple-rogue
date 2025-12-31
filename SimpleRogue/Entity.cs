namespace SimpleRogue;

public abstract class Entity(Position position, char symbol, int maxHealth, int attack, string name)
{
    public Position Position { get; set; } = position;
    public char Symbol { get; } = symbol;
    public int Health { get; set; } = maxHealth;
    public int MaxHealth { get; } = maxHealth;
    public int Attack { get; } = attack;
    public string Name { get; } = name;

    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage) => Health = Math.Max(0, Health - damage);
    public void Heal(int amount) => Health = Math.Min(MaxHealth, Health + amount);
}

public class Player(Position position) : Entity(position, '@', 100, 10, "Player")
{
    public int Gold { get; set; }
}

public class Enemy(Position position, char symbol, int maxHealth, int attack, string name) 
    : Entity(position, symbol, maxHealth, attack, name)
{
    // Enemy definitions: (symbol, health, attack, name)
    private static readonly (char Symbol, int Health, int Attack, string Name)[] EnemyTypes =
    [
        ('g', 30, 5, "Goblin"),
        ('O', 50, 8, "Orc"),
        ('T', 80, 12, "Troll")
    ];

    public static Enemy CreateRandom(Position position, Random random)
    {
        var (symbol, health, attack, name) = EnemyTypes[random.Next(EnemyTypes.Length)];
        return new Enemy(position, symbol, health, attack, name);
    }
}
