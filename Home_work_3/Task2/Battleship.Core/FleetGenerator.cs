namespace Battleship.Core;

public class FleetGenerator
{
    private readonly ShipPlacementValidator _validator;

    public FleetGenerator(ShipPlacementValidator validator)
    {
        _validator = validator;
    }

    public void GenerateRandomFleet(
        Board board,
        IReadOnlyList<ShipSpec> shipSpecs,
        int? seed = null)
    {
        if (shipSpecs.Count == 0)
            throw new ArgumentException("Ship specs list must not be empty.", nameof(shipSpecs));

        var shipLengths = shipSpecs
            .SelectMany(spec => Enumerable.Repeat(spec.Size, spec.Count))
            .OrderByDescending(x => x)
            .ToArray();

        if (shipLengths.Any(length => length <= 0))
            throw new ArgumentException("All ship lengths must be positive.", nameof(shipSpecs));

        var random = seed.HasValue
            ? new Random(seed.Value)
            : new Random();

        for (var attempt = 0; attempt < 200; attempt++)
        {
            board.Ships.Clear();
            board.Shots.Clear();

            var success = true;

            foreach (var shipLength in shipLengths)
            {
                if (!TryPlaceRandomShip(board, shipLength, random))
                {
                    success = false;
                    break;
                }
            }

            if (success)
                return;
        }

        throw new InvalidOperationException(
            "Failed to generate fleet for the current board size.");
    }

    public bool TryPlaceRandomShip(
        Board board,
        int length,
        Random random)
    {
        for (var attempt = 0; attempt < 1000; attempt++)
        {
            var horizontal = random.Next(0, 2) == 0;

            var startRow = horizontal
                ? random.Next(0, board.Size)
                : random.Next(0, board.Size - length + 1);

            var startColumn = horizontal
                ? random.Next(0, board.Size - length + 1)
                : random.Next(0, board.Size);

            var cells = new List<Position>(length);

            for (var i = 0; i < length; i++)
            {
                var row = horizontal
                    ? startRow
                    : startRow + i;

                var column = horizontal
                    ? startColumn + i
                    : startColumn;

                cells.Add(new Position(row, column));
            }

            if (!_validator.CanPlaceShip(board, cells))
                continue;

            board.Ships.Add(new Ship(cells));
            return true;
        }

        return false;
    }
}