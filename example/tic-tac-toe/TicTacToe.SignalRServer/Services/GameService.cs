using TicTacToe.Interfaces.Grains;
using TicTacToe.Shared.Models;

namespace TicTacToe.SignalRServer.Services;

public class GameService
{
    private readonly IClusterClient clusterClient;

    public GameService(IClusterClient clusterClient)
    {
        this.clusterClient = clusterClient;
    }

    public Task<GameState> GetGameStateAsync(string gameId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.GetGameStateAsync();
    }

    public Task AttemptPlayAsync(string gameId, string userId, Play play)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.AttemptPlayAsync(userId, play);
    }

    public Task<Symbol> JoinGameAsync(string gameId, string userId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.JoinGameAsync(gameId, userId);
    }
}
