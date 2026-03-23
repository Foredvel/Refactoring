namespace Battleship.Core;

public class VictoryMessage
{
    public Lazy<string> Message = new(() => "All ships are sunk. You win.");
}
