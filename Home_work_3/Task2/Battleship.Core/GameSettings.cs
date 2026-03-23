namespace Battleship.Core;
using BattleshipGame;

public class GameSettings
{
    public int BoardSize = 10;
    public List<ShipSpec> Fleet = new();

    public GameSettings(int boardSize)
    {
        BoardSize = boardSize;
        Fleet = FleetFactory.CreateForBoardSize(boardSize);
    }
}
