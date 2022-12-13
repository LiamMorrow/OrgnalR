using Orleans;

namespace TicTacToe.Shared.Models;

[GenerateSerializer]
public enum Symbol
{
    [Id(0)]
    X,

    [Id(1)]
    O
}

[GenerateSerializer]
public record Coord(int Row, int Column);

[GenerateSerializer]
public record Play(Symbol Symbol, Coord Coord);

[GenerateSerializer]
public record GameState(Symbol Turn, Symbol?[,] Grid, Symbol? Winner, bool Draw);
