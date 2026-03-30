namespace Battleship.Core;


public class GameEngine
{
    private readonly ShipPlacementValidator _validator;

    public GameEngine(ShipPlacementValidator validator)
    {
        _validator = validator;
    }

    public string Fire(Board board, Position position)
    {
        if (!_validator.IsInBounds(board, position))
            return ShotResults.OutOfBounds;

        if (!board.Shots.Add(position))
            return ShotResults.AlreadyShot;

        foreach (var ship in board.Ships)
        {
            if (!ship.RegisterHit(position))
                continue;

            return ship.IsSunk()
                ? ShotResults.Sunk
                : ShotResults.Hit;
        }

        return ShotResults.Miss;
    }

    public bool AllShipsSunk(Board board)
    {
        return board.Ships.Count > 0 &&
               board.Ships.All(x => x.IsSunk());
    }
}