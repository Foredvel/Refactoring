namespace LifeSim;

public readonly record struct Point2(int X, int Y)
{

    public static bool AreNeighborsOrSame(Point2 a, Point2 b) =>
        Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;
}
