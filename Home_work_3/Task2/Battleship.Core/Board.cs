namespace Battleship.Core;

public class Board
{
    public List<Ship> Ships = new();
    public HashSet<Position> Shots = new();
    public int Size;

    public Board(int size = 10)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Board size must be positive.");
        }

        Size = size;
    }

    public bool AllShipsSunk()
    {
        return Ships.Count > 0 && Ships.All(x => x.IsSunk());
    }

    public void PlaceShip(Ship ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        if (!CanPlaceShip(ship.Cells))
        {
            throw new InvalidOperationException("Ship placement violates board rules.");
        }

        Ships.Add(ship);
    }

    public bool CanPlaceShip(IEnumerable<Position> cells)
    {
        var normalized = cells.Distinct().ToList();
        if (normalized.Count == 0)
        {
            return false;
        }

        if (normalized.Any(cell => !IsInBounds(cell)))
        {
            return false;
        }

        foreach (var cell in normalized)
        {
            foreach (var existing in Ships)
            {
                foreach (var existingCell in existing.Cells)
                {
                    if (Math.Abs(existingCell.Row - cell.Row) <= 1 && Math.Abs(existingCell.Column - cell.Column) <= 1)
                    {
                        return false;
                    }
                }
            }
        }

        return IsStraightLine(normalized);
    }

    public void GenerateRandomFleet(IReadOnlyList<int> shipLengths, int? seed = null)
    {
        if (shipLengths.Count == 0)
        {
            throw new ArgumentException("Ship lengths list must not be empty.", nameof(shipLengths));
        }

        if (shipLengths.Any(length => length <= 0))
        {
            throw new ArgumentException("All ship lengths must be positive.", nameof(shipLengths));
        }

        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var sortedLengths = shipLengths.OrderByDescending(x => x).ToArray();

        for (var attempt = 0; attempt < 200; attempt++)
        {
            Ships.Clear();
            Shots.Clear();
            var success = true;

            foreach (var shipLength in sortedLengths)
            {
                if (!TryPlaceRandomShip(shipLength, random))
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                return;
            }
        }

        throw new InvalidOperationException("Failed to generate fleet for the current board size.");
    }

    public string Fire(Position position)
    {
        if (!IsInBounds(position))
        {
            return ShotResults.OutOfBounds;
        }

        if (!Shots.Add(position))
        {
            return ShotResults.AlreadyShot;
        }

        foreach (var ship in Ships)
        {
            if (!ship.RegisterHit(position))
            {
                continue;
            }

            return ship.IsSunk() ? ShotResults.Sunk : ShotResults.Hit;
        }

        return ShotResults.Miss;
    }

    public bool IsInBounds(Position position)
    {
        return position.Row >= 0
               && position.Row < Size
               && position.Column >= 0
               && position.Column < Size;
    }

    private bool TryPlaceRandomShip(int length, Random random)
    {
        for (var attempt = 0; attempt < 1000; attempt++)
        {
            var horizontal = random.Next(0, 2) == 0;
            var startRow = horizontal ? random.Next(0, Size) : random.Next(0, Size - length + 1);
            var startColumn = horizontal ? random.Next(0, Size - length + 1) : random.Next(0, Size);
            var cells = new List<Position>(length);

            for (var i = 0; i < length; i++)
            {
                var row = horizontal ? startRow : startRow + i;
                var column = horizontal ? startColumn + i : startColumn;
                cells.Add(new Position(row, column));
            }

            if (!CanPlaceShip(cells))
            {
                continue;
            }

            Ships.Add(new Ship(cells));
            return true;
        }

        return false;
    }

    private static bool IsStraightLine(List<Position> cells)
    {
        if (cells.Count == 1)
        {
            return true;
        }

        var sameRow = cells.All(x => x.Row == cells[0].Row);
        var sameColumn = cells.All(x => x.Column == cells[0].Column);

        if (!sameRow && !sameColumn)
        {
            return false;
        }

        if (sameRow)
        {
            var ordered = cells.Select(x => x.Column).OrderBy(x => x).ToArray();
            for (var i = 1; i < ordered.Length; i++)
            {
                if (ordered[i] - ordered[i - 1] != 1)
                {
                    return false;
                }
            }

            return true;
        }

        var rows = cells.Select(x => x.Row).OrderBy(x => x).ToArray();
        for (var i = 1; i < rows.Length; i++)
        {
            if (rows[i] - rows[i - 1] != 1)
            {
                return false;
            }
        }

        return true;
    }
}
