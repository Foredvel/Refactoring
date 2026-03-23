using Battleship.Core;

var tests = new (string Name, Action Test)[]
{
    ("Fire_ReturnsOutOfBounds_ForOutsidePosition", Fire_ReturnsOutOfBounds_ForOutsidePosition),
    ("Fire_ReturnsHitThenSunk_ForShipCells", Fire_ReturnsHitThenSunk_ForShipCells),
    ("Fire_ReturnsAlreadyShot_ForRepeatedPosition", Fire_ReturnsAlreadyShot_ForRepeatedPosition),
    ("PlaceShip_Throws_ForOverlappingShips", PlaceShip_Throws_ForOverlappingShips),
    ("PlaceShip_Throws_ForDiagonalTouchingShips", PlaceShip_Throws_ForDiagonalTouchingShips),
    ("GenerateRandomFleet_CreatesExpectedShipCount", GenerateRandomFleet_CreatesExpectedShipCount),
    ("GenerateRandomFleet_HasNoTouchingShips", GenerateRandomFleet_HasNoTouchingShips)
};

var failed = 0;
foreach (var (name, test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"[PASS] {name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"[FAIL] {name}: {ex.Message}");
    }
}

Console.WriteLine($"Total: {tests.Length}, Failed: {failed}");
return failed == 0 ? 0 : 1;

static void Fire_ReturnsOutOfBounds_ForOutsidePosition()
{
    var board = new Board(3);
    var result = board.Fire(new Position(-1, 0));
    AssertEqual(ShotResults.OutOfBounds, result, "Expected out of bounds for negative row.");
}

static void Fire_ReturnsHitThenSunk_ForShipCells()
{
    var board = new Board(3);
    board.PlaceShip(new Ship(new[] { new Position(0, 0), new Position(0, 1) }));

    var first = board.Fire(new Position(0, 0));
    var second = board.Fire(new Position(0, 1));

    AssertEqual(ShotResults.Hit, first, "First shot should be hit.");
    AssertEqual(ShotResults.Sunk, second, "Second shot should sink ship.");
    AssertTrue(board.AllShipsSunk(), "AllShipsSunk should be true.");
}

static void Fire_ReturnsAlreadyShot_ForRepeatedPosition()
{
    var board = new Board(3);
    var first = board.Fire(new Position(1, 1));
    var second = board.Fire(new Position(1, 1));

    AssertEqual(ShotResults.Miss, first, "First shot should miss.");
    AssertEqual(ShotResults.AlreadyShot, second, "Second shot should be marked as already shot.");
}

static void PlaceShip_Throws_ForOverlappingShips()
{
    var board = new Board(3);
    board.PlaceShip(new Ship(new[] { new Position(1, 1) }));

    AssertThrows<InvalidOperationException>(() => board.PlaceShip(new Ship(new[] { new Position(1, 1) })));
}

static void PlaceShip_Throws_ForDiagonalTouchingShips()
{
    var board = new Board(5);
    board.PlaceShip(new Ship(new[] { new Position(1, 1) }));

    AssertThrows<InvalidOperationException>(() => board.PlaceShip(new Ship(new[] { new Position(2, 2) })));
}

static void GenerateRandomFleet_CreatesExpectedShipCount()
{
    var board = new Board(10);
    var fleet = FleetFactory.CreateForBoardSize(10);
    board.GenerateRandomFleet(fleet, seed: 1);

    AssertEqual(fleet.Count, board.Ships.Count, "Fleet ship count must match.");
    AssertEqual(fleet.Sum(), board.Ships.Sum(x => x.Cells.Count), "Total ship cells must match.");
}

static void GenerateRandomFleet_HasNoTouchingShips()
{
    var board = new Board(10);
    board.GenerateRandomFleet(FleetFactory.CreateForBoardSize(10), seed: 2);

    for (var i = 0; i < board.Ships.Count; i++)
    {
        for (var j = i + 1; j < board.Ships.Count; j++)
        {
            foreach (var a in board.Ships[i].Cells)
            {
                foreach (var b in board.Ships[j].Cells)
                {
                    var touches = Math.Abs(a.Row - b.Row) <= 1 && Math.Abs(a.Column - b.Column) <= 1;
                    if (touches)
                    {
                        throw new InvalidOperationException($"Ships touch at {a} and {b}.");
                    }
                }
            }
        }
    }
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{message} Expected: {expected}, Actual: {actual}.");
    }
}

static void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertThrows<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected exception {typeof(TException).Name} was not thrown.");
}
