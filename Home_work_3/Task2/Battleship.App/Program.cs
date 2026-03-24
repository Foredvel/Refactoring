using Battleship.Core;


var boardSize = ParseBoardSize(args);
var settings = new GameSettings(boardSize);
var board = new Board(size: settings.BoardSize);
var validator = new ShipPlacementValidator();
var generator = new FleetGenerator(validator);
generator.GenerateRandomFleet(board, settings.Fleet);

var engine = new GameEngine(validator);
var game = new Game(board, engine);
var shotHistory = new Dictionary<Position, string>();
var boardLegend = new BoardLegend();

Console.WriteLine("Battleship demo started.");
Console.WriteLine($"Board size: {boardSize}x{boardSize}. Enter coordinates as: row col (for example: 0 1).");
Console.WriteLine($"Fleet: {string.Join(", ", settings.Fleet)}");
Console.WriteLine("Type 'q' to exit.");


//Переделать на команду 
while (true)
{
    if (game.IsGameOver())
    {
        Console.WriteLine("All ships are sunk. You win.");
        PrintBoard(game.Board, shotHistory);
        PrintLegend(boardLegend.Legend.Value);
        break;
    }

    Console.Write("Shot> ");
    var input = Console.ReadLine();

    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Exit.");
        PrintBoardOnExit(game.Board, shotHistory, boardLegend.Legend.Value);
        break;
    }

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Empty input. Use format: row col.");
        continue;
    }

    if (!TryParsePosition(input, out var shotPosition))
    {
        Console.WriteLine("Invalid format. Use two integers: row col.");
        continue;
    }

    var result = game.MakeShot(shotPosition);
    shotHistory[shotPosition] = result;
    Console.WriteLine($"Result: {result}");
}

static void PrintBoard(Board board, IReadOnlyDictionary<Position, string> shots)
{
    Console.WriteLine("Final board:");
    Console.Write("   ");
    for (var c = 0; c < board.Size; c++)
    {
        Console.Write($"{c} ");
    }

    Console.WriteLine();

    for (var r = 0; r < board.Size; r++)
    {
        Console.Write($"{r,2} ");
        for (var c = 0; c < board.Size; c++)
        {
            var position = new Position(r, c);
            var cell = GetCellSymbol(board, shots, position);
            Console.Write($"{cell} ");
        }

        Console.WriteLine();
    }
}

static char GetCellSymbol(Board board, IReadOnlyDictionary<Position, string> shots, Position position)
{
    var hasShip = board.Ships.Any(s => s.Occupies(position)); //посмотреть что можно засунуть внутрь
    var hasShot = shots.TryGetValue(position, out var result);

    if (hasShip)
    {
        return 'X';
    }

    return hasShot && result == ShotResults.Miss ? 'o' : '~';
}

static void PrintLegend(IReadOnlyDictionary<char, string> legend)
{
    Console.WriteLine("Legend:");
    foreach (var item in legend)
    {
        Console.WriteLine($"  {item.Key}: {item.Value}");
    }
}

static int ParseBoardSize(string[] args)
{
    if (args.Length == 0)
    {
        return 10;
    }

    if (int.TryParse(args[0], out var size) && size >= 5)
    {
        return size;
    }

    Console.WriteLine("Invalid board size argument. Using default size 10.");
    return 10;
}

static void PrintBoardOnExit(Board board, IReadOnlyDictionary<Position, string> shots, IReadOnlyDictionary<char, string> legend)
{
    Console.WriteLine("Board on exit:");

    var limit = board.Size;
    var col = 0;
    Console.Write("   ");
    while (col < limit)
    {
        Console.Write(col);
        Console.Write(" ");
        col = col + 1;
    }

    Console.WriteLine();

    for (var row = 0; row < board.Size; row++)
    {
        if (row < 10)
        {
            Console.Write(" ");
            Console.Write(row);
            Console.Write(" ");
        }
        else
        {
            Console.Write(row);
            Console.Write(" ");
        }

        for (var colum = 0; colum < board.Size; colum++)
        {
            var position = new Position(row, colum);
            var shipOnCell = false;
            foreach (var shipInALoop in board.Ships)
            {
                if (shipInALoop.Occupies(position))
                {
                    shipOnCell = true;
                }
            }

            var result = shots.TryGetValue(position, out _);
            char charCell;
            if (shipOnCell)
            {
                if (result)
                {
                    charCell = 'x';
                }
                else
                {
                    charCell = 'X';
                }
            }
            else
            {
                if (result)
                {
                    charCell = 'o';
                }
                else
                {
                    charCell = '~';
                }
            }

            Console.Write(charCell);
            Console.Write(" ");
        }

        Console.WriteLine();
    }

    Console.WriteLine("Legend on exit:");
    foreach (var pair in legend)
    {
        Console.WriteLine($"  {pair.Key}: {pair.Value}");
    }
    Console.WriteLine("  x: hit");
}

static bool TryParsePosition(string input, out Position pos)
{
    pos = default;
    if (string.IsNullOrWhiteSpace(input)) return false;
    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (parts.Length != 2 || !int.TryParse(parts[0], out var row) || !int.TryParse(parts[1], out var column))
    {
        Console.WriteLine("Invalid format. Use two integers: row col.");
        return true;
    }
    return false;
}
