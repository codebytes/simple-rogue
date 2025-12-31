namespace SimpleRogue;

public abstract class Item(Position position, char symbol, string name)
{
    public Position Position { get; set; } = position;
    public char Symbol { get; } = symbol;
    public string Name { get; } = name;

    public abstract void Apply(Player player);
    public abstract string GetPickupMessage();
}

public class HealthPotion(Position position, int healAmount = 30) : Item(position, '!', "Health Potion")
{
    public override void Apply(Player player) => player.Heal(healAmount);
    public override string GetPickupMessage() => $"You picked up a {Name} and restored health!";
}

public class Gold(Position position, int amount = 10) : Item(position, '$', $"{amount} Gold")
{
    public override void Apply(Player player) => player.Gold += amount;
    public override string GetPickupMessage() => $"You picked up {Name}!";
}
