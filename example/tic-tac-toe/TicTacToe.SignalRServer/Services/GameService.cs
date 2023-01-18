using System.Reflection.Metadata.Ecma335;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Shared.Models;
using TicTacToe.SignalRServer.Models;

namespace TicTacToe.SignalRServer.Services;

public class GameService
{
    private readonly IClusterClient clusterClient;

    public GameService(IClusterClient clusterClient)
    {
        this.clusterClient = clusterClient;
    }

    public async Task<ConnectedGame> GetGameStateAsync(string gameId, string userId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);
        var state = await gameGrain.GetGameStateAsync();
        var players = await gameGrain.GetPlayersAsync();
        var player = players.FirstOrDefault(x => x.Id == userId);
        var opponent = players.FirstOrDefault(x => x.Id != userId);

        if (player == null)
        {
            throw new Exception("Player not a part of this game.");
        }

        return new ConnectedGame(state, player.Mark, opponent != null ? new(opponent.IsBot) : null);
    }

    public Task AttemptPlayAsync(string gameId, string userId, Play play)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.AttemptPlayAsync(userId, play);
    }

    public Task<Mark> JoinGameAsync(string gameId, string userId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.JoinGameAsync(userId);
    }

    public Task AddBotAsync(string gameId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        return gameGrain.AddBotAsync();
    }
}
