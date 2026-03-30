namespace Battleship.Core;


public class ShipPlacementValidator
{
    public bool CanPlaceShip(Board board, IEnumerable<Position> cells)
    {
        var normalized = cells.Distinct().ToList();

        if (normalized.Count == 0)
            return false;

        if (normalized.Any(cell => !IsInBounds(board, cell)))
            return false;

        foreach (var cell in normalized)
        {
            foreach (var existing in board.Ships)
            {
                foreach (var existingCell in existing.Cells)
                {
                    if (Math.Abs(existingCell.Row - cell.Row) <= 1 &&
                        Math.Abs(existingCell.Column - cell.Column) <= 1)
                    {
                        return false;
                    }
                }
            }
        }

        return IsStraightLine(normalized);
    }

    public bool IsInBounds(Board board, Position position)
    {
        return position.Row >= 0 &&
               position.Row < board.Size &&
               position.Column >= 0 &&
               position.Column < board.Size;
    }

    public bool IsStraightLine(List<Position> cells)
    {
        if (cells.Count == 1)
            return true;

        var sameRow = cells.All(x => x.Row == cells[0].Row);
        var sameColumn = cells.All(x => x.Column == cells[0].Column);

        if (!sameRow && !sameColumn)
            return false;

        if (sameRow)
        {
            var ordered = cells.Select(x => x.Column).OrderBy(x => x).ToArray();

            for (var i = 1; i < ordered.Length; i++)
            {
                if (ordered[i] - ordered[i - 1] != 1)
                    return false;
            }

            return true;
        }

        var rows = cells.Select(x => x.Row).OrderBy(x => x).ToArray();

        for (var i = 1; i < rows.Length; i++)
        {
            if (rows[i] - rows[i - 1] != 1)
                return false;
        }

        return true;
    }
}