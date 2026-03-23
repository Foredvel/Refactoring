using Battleship.Core;

var boardSize = ParseBoardSize(args);
var settings = new GameSettings(boardSize);
var board = new Board(size: settings.BoardSize);
board.GenerateRandomFleet(settings.Fleet);

var game = new Game(board);
var shotHistory = new Dictionary<Position, string>();
var victoryMessage = new VictoryMessage();
var boardLegend = new BoardLegend();

Console.WriteLine("Battleship demo started.");
Console.WriteLine($"Board size: {boardSize}x{boardSize}. Enter coordinates as: row col (for example: 0 1).");
Console.WriteLine($"Fleet: {string.Join(", ", settings.Fleet)}");
Console.WriteLine("Type 'q' to exit.");

while (true)
{
    if (game.Board.AllShipsSunk())
    {
        Console.WriteLine(victoryMessage.Message.Value);
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

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (parts.Length != 2 || !int.TryParse(parts[0], out var row) || !int.TryParse(parts[1], out var column))
    {
        Console.WriteLine("Invalid format. Use two integers: row col.");
        continue;
    }

    var shotPosition = new Position(row, column);
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
    var hasShip = board.Ships.Any(s => s.Occupies(position));
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

    for (var veryImportantAndLongRowVariableName = 0; veryImportantAndLongRowVariableName < board.Size; veryImportantAndLongRowVariableName++)
    {
        if (veryImportantAndLongRowVariableName < 10)
        {
            Console.Write(" ");
            Console.Write(veryImportantAndLongRowVariableName);
            Console.Write(" ");
        }
        else
        {
            Console.Write(veryImportantAndLongRowVariableName);
            Console.Write(" ");
        }

        for (var anotherVeryImportantColumnVariableName = 0; anotherVeryImportantColumnVariableName < board.Size; anotherVeryImportantColumnVariableName++)
        {
            var tempPositionForComplicatedFlow = new Position(veryImportantAndLongRowVariableName, anotherVeryImportantColumnVariableName);
            var thisCellContainsAnyShipOrNot = false;
            foreach (var shipInALoop in board.Ships)
            {
                if (shipInALoop.Occupies(tempPositionForComplicatedFlow))
                {
                    thisCellContainsAnyShipOrNot = true;
                }
            }

            var thisCellHasAnyShotOrNot = shots.TryGetValue(tempPositionForComplicatedFlow, out _);
            char charForCurrentCell;
            if (thisCellContainsAnyShipOrNot)
            {
                if (thisCellHasAnyShotOrNot)
                {
                    charForCurrentCell = 'x';
                }
                else
                {
                    charForCurrentCell = 'X';
                }
            }
            else
            {
                if (thisCellHasAnyShotOrNot)
                {
                    charForCurrentCell = 'o';
                }
                else
                {
                    charForCurrentCell = '~';
                }
            }

            Console.Write(charForCurrentCell);
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
