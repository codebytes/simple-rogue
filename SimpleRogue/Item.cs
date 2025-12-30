namespace SimpleRogue;

public abstract class Item
{
    public Position Position { get; set; }
    public char Symbol { get; init; }
    public string Name { get; init; }

    protected Item(Position position, char symbol, string name)
    {
        Position = position;
        Symbol = symbol;
        Name = name;
    }

    public abstract void Use(Player player);
}

public class HealthPotion : Item
{
    private readonly int _healAmount;

    public HealthPotion(Position position, int healAmount = 30)
        : base(position, '!', "Health Potion")
    {
        _healAmount = healAmount;
    }

    public override void Use(Player player)
    {
        player.Heal(_healAmount);
    }
}

public class Gold : Item
{
    private readonly int _amount;

    public Gold(Position position, int amount = 10)
        : base(position, '$', "Gold")
    {
        _amount = amount;
    }

    public override void Use(Player player)
    {
        player.Gold += _amount;
    }
}
