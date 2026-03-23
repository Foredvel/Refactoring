namespace Battleship.Core;

public struct Position
{
    public int Row;
    public int Column;

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public override string ToString() => $"({Row},{Column})";

    public override bool Equals(object? obj)
    {
        return obj is Position other && Row == other.Row && Column == other.Column;
    }

    public override int GetHashCode() => HashCode.Combine(Row, Column);
}
