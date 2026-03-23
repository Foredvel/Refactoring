namespace Battleship.Core;

public class Game
{
    public Board Board;

    public Game(Board board)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
    }

    public string MakeShot(Position position)
    {
        return Board.Fire(position);
    }
}
