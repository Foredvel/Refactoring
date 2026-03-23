namespace Battleship.Core;

public class GameSettings
{
    public int BoardSize = 10;
    public List<int> Fleet = new();

    public GameSettings(int boardSize)
    {
        BoardSize = boardSize;
        Fleet = FleetFactory.CreateForBoardSize(boardSize);
    }
}
