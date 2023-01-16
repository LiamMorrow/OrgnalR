using Orleans.Runtime;
using TicTacToe.Engine;
using TicTacToe.Interfaces.Grains;
using TicTacToe.OrleansSilo.Service;
using TicTacToe.Shared.Models;

namespace TicTacToe.OrleansSilo.GrainImplementations;

public class GameGrain : IGameGrain, IGrainBase
{
    public IGrainContext GrainContext { get; }

    private readonly Game game = new();
    private readonly Dictionary<Mark, string> users = new();
    private readonly IEnumerable<IGameStateNotifier> gameStateNotifiers;

    public GameGrain(IGrainContext grainContext, IEnumerable<IGameStateNotifier> gameStateNotifiers)
    {
        this.GrainContext = grainContext;
        this.gameStateNotifiers = gameStateNotifiers;
    }

    public Task<Mark> JoinGameAsync(string gameId, string userId)
    {
        if (!users.TryGetValue(Mark.X, out _))
        {
            users[Mark.X] = userId;
            return Task.FromResult(Mark.X);
        }

        if (!users.TryGetValue(Mark.O, out _))
        {
            users[Mark.O] = userId;
            return Task.FromResult(Mark.O);
        }

        throw new InvalidOperationException("No players left in game");
    }

    public Task<GameState> GetGameStateAsync()
    {
        return Task.FromResult(game.CurrentState);
    }

    public Task AttemptPlayAsync(string userId, Play play)
    {
        if (!users.TryGetValue(play.Mark, out var markUser))
        {
            throw new InvalidOperationException("No player assigned to mark " + play.Mark);
        }
        if (markUser != userId)
        {
            throw new InvalidOperationException("Player not assigned to mark " + play.Mark);
        }

        game.AttemptPlay(play);

        foreach (var gameStateNotifier in gameStateNotifiers)
        {
            gameStateNotifier.NotifyNewGameStateAvailable(this.GetPrimaryKeyString());
        }

        return Task.CompletedTask;
    }
}
