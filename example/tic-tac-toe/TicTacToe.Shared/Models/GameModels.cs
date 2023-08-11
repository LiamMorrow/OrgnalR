using Orleans;

namespace TicTacToe.Shared.Models;

[GenerateSerializer]
public enum Mark
{
    [Id(0)]
    X,

    [Id(1)]
    O
}

[GenerateSerializer]
public record Coord(int Row, int Column);

[GenerateSerializer]
public record Play(Mark Mark, Coord Coord);

[GenerateSerializer]
public record GameState(Mark Turn, Mark?[][] Grid, Mark? Winner, bool Draw)
{
    public bool GameOver => Draw || Winner != null;
}

[GenerateSerializer]
public record ConnectedPlayer(string Id, Mark Mark, bool IsBot);
