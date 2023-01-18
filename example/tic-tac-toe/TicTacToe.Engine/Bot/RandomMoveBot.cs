using Orleans.Runtime;
using TicTacToe.Shared.Models;

namespace TicTacToe.Engine.Bot;

public class RandomMoveBot : IBot
{
    private readonly Random random;

    public RandomMoveBot(Random random)
    {
        this.random = random;
    }

    public Play GetNextPlay(GameState gameState)
    {
        return gameState.Grid
            .SelectMany(
                (row, rowNum) =>
                    row.Where(mark => mark == null)
                        .Select((_, colNum) => new Play(gameState.Turn, new Coord(rowNum, colNum)))
            )
            .Where(play => play != null)
            .OrderBy(_ => random.Next())
            .First();
    }
}
