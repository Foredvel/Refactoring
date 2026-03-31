namespace LifeSim;

public readonly record struct Point2(int X, int Y)
{
    public override string ToString() => $"({X},{Y})"; //тереотическая общность, то нигде не используется 

    public static bool AreNeighborsOrSame(Point2 a, Point2 b) =>
        Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;
}
