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
                    row.Select(
                            (mark, colNum) =>
                                new
                                {
                                    Play = new Play(gameState.Turn, new Coord(rowNum, colNum)),
                                    ExistingMark = mark
                                }
                        )
                        .Where(play => play.ExistingMark == null)
                        .Select(x => x.Play)
            )
            .OrderBy(_ => random.Next())
            .First();
    }
}
