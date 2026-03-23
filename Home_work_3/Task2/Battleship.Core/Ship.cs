namespace Battleship.Core;

public class Ship
{
    public HashSet<Position> Cells = new();
    public HashSet<Position> Hits = new();

    public Ship(IEnumerable<Position> cells)
    {
        Cells = cells.ToHashSet();

        if (Cells.Count == 0)
        {
            throw new ArgumentException("Ship must have at least one cell.", nameof(cells));
        }
    }

    public bool IsSunk()
    {
        return Hits.Count == Cells.Count;
    }

    public bool Occupies(Position position)
    {
        return Cells.Contains(position);
    }

    public bool RegisterHit(Position position)
    {
        if (!Cells.Contains(position))
        {
            return false;
        }

        Hits.Add(position);
        return true;
    }
}
