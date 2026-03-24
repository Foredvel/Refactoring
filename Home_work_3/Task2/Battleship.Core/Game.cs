namespace Battleship.Core;

public class Game
{
    public Board Board { get; }

    private readonly GameEngine _engine;

    public Game(Board board, GameEngine engine)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
    }

    public string MakeShot(Position position)
    {
        return _engine.Fire(Board, position);
    }

    public bool IsGameOver()
    {
        return _engine.AllShipsSunk(Board);
    }
}
