namespace SimpleRogue;

public record Position(int X, int Y)
{
    public Position Move(int dx, int dy) => new(X + dx, Y + dy);
}
