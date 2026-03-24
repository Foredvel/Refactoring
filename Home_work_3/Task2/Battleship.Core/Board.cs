namespace Battleship.Core;

public class Board
{
    public List<Ship> Ships { get; } = new();
    public HashSet<Position> Shots { get; } = new();
    public int Size { get; }

    public Board(int size = 10)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Board size must be positive.");

        Size = size;
    }
}

