namespace Battleship.Core;

public static class FleetFactory
{
    public static List<int> CreateForBoardSize(int size)
    {
        if (size < 5)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Minimum supported board size is 5.");
        }

        if (size >= 10)
        {
            return new List<int> { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        }

        if (size >= 8)
        {
            return new List<int> { 4, 3, 3, 2, 2, 1, 1 };
        }

        if (size >= 6)
        {
            return new List<int> { 3, 2, 2, 1, 1 };
        }

        return new List<int> { 3, 2, 1, 1 };
    }
}
