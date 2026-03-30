namespace Battleship.Core;

using BattleshipGame;

public readonly record struct ShipSpec(int Size, int Count);

public static class FleetFactory
{
    public static List<ShipSpec> CreateForBoardSize(int size)
    {
        if (size < 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "Minimum supported board size is 5."
            );
        }

        if (size >= 10)
        {
            return new()
            {
                new ShipSpec(4, 1),
                new ShipSpec(3, 2),
                new ShipSpec(2, 3),
                new ShipSpec(1, 4)
            };
        }

        if (size >= 8)
        {
            return new()
            {
                new ShipSpec(4, 1),
                new ShipSpec(3, 2),
                new ShipSpec(2, 2),
                new ShipSpec(1, 2)
            };
        }

        if (size >= 6)
        {
            return new()
            {
                new ShipSpec(3, 1),
                new ShipSpec(2, 2),
                new ShipSpec(1, 2)
            };
        }

        return new()
        {
            new ShipSpec(3, 1),
            new ShipSpec(2, 1),
            new ShipSpec(1, 2)
        };
    }
}