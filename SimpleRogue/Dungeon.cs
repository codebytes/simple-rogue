namespace SimpleRogue;

public enum TileType
{
    Floor,
    Wall,
    Door
}

public class Tile
{
    public TileType Type { get; set; }
    public bool IsWalkable => Type == TileType.Floor || Type == TileType.Door;

    public char GetSymbol() => Type switch
    {
        TileType.Floor => '.',
        TileType.Wall => '#',
        TileType.Door => '+',
        _ => ' '
    };
}

public class Dungeon
{
    private readonly Tile[,] _tiles;
    public int Width { get; }
    public int Height { get; }

    public Dungeon(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width, height];
        InitializeTiles();
        GenerateDungeon();
    }

    private void InitializeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _tiles[x, y] = new Tile { Type = TileType.Wall };
            }
        }
    }

    private void GenerateDungeon()
    {
        var random = new Random();
        var rooms = new List<Rectangle>();

        // Generate rooms
        const int maxRooms = 8;
        const int minRoomSize = 5;
        const int maxRoomSize = 10;

        for (int i = 0; i < maxRooms; i++)
        {
            int width = random.Next(minRoomSize, maxRoomSize + 1);
            int height = random.Next(minRoomSize, maxRoomSize + 1);
            int x = random.Next(1, Width - width - 1);
            int y = random.Next(1, Height - height - 1);

            var newRoom = new Rectangle(x, y, width, height);
            bool overlaps = rooms.Any(room => newRoom.Intersects(room));

            if (!overlaps)
            {
                CreateRoom(newRoom);

                if (rooms.Count > 0)
                {
                    var prevRoom = rooms[^1];
                    if (random.Next(2) == 0)
                    {
                        CreateHorizontalTunnel(prevRoom.CenterX, newRoom.CenterX, prevRoom.CenterY);
                        CreateVerticalTunnel(prevRoom.CenterY, newRoom.CenterY, newRoom.CenterX);
                    }
                    else
                    {
                        CreateVerticalTunnel(prevRoom.CenterY, newRoom.CenterY, prevRoom.CenterX);
                        CreateHorizontalTunnel(prevRoom.CenterX, newRoom.CenterX, newRoom.CenterY);
                    }
                }

                rooms.Add(newRoom);
            }
        }
    }

    private void CreateRoom(Rectangle room)
    {
        for (int x = room.X; x < room.X + room.Width; x++)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                if (IsInBounds(x, y))
                {
                    _tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    private void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        int start = Math.Min(x1, x2);
        int end = Math.Max(x1, x2);

        for (int x = start; x <= end; x++)
        {
            if (IsInBounds(x, y))
            {
                _tiles[x, y].Type = TileType.Floor;
            }
        }
    }

    private void CreateVerticalTunnel(int y1, int y2, int x)
    {
        int start = Math.Min(y1, y2);
        int end = Math.Max(y1, y2);

        for (int y = start; y <= end; y++)
        {
            if (IsInBounds(x, y))
            {
                _tiles[x, y].Type = TileType.Floor;
            }
        }
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public bool IsWalkable(Position position) => 
        IsInBounds(position.X, position.Y) && _tiles[position.X, position.Y].IsWalkable;

    public Tile GetTile(Position position) => 
        IsInBounds(position.X, position.Y) ? _tiles[position.X, position.Y] : new Tile { Type = TileType.Wall };

    public Position GetRandomFloorPosition()
    {
        var random = new Random();
        int attempts = 0;
        const int maxAttempts = 1000;

        while (attempts < maxAttempts)
        {
            int x = random.Next(Width);
            int y = random.Next(Height);
            var pos = new Position(x, y);

            if (IsWalkable(pos))
            {
                return pos;
            }

            attempts++;
        }

        // Fallback: find first walkable tile
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var pos = new Position(x, y);
                if (IsWalkable(pos))
                {
                    return pos;
                }
            }
        }

        return new Position(Width / 2, Height / 2);
    }

    private record Rectangle(int X, int Y, int Width, int Height)
    {
        public int CenterX => X + Width / 2;
        public int CenterY => Y + Height / 2;

        public bool Intersects(Rectangle other)
        {
            return X < other.X + other.Width &&
                   X + Width > other.X &&
                   Y < other.Y + other.Height &&
                   Y + Height > other.Y;
        }
    }
}
