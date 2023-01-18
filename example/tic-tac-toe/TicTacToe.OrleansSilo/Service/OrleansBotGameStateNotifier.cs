using OrgnalR.Core.Provider;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Interfaces.HubClients;
using TicTacToe.Interfaces.Hubs;

namespace TicTacToe.OrleansSilo.Service;

public class OrleansBotGameStateNotifier : IGameStateNotifier
{
    private readonly IClusterClient clusterClient;

    public OrleansBotGameStateNotifier(IClusterClient clusterClient)
    {
        this.clusterClient = clusterClient;
    }

    public async void NotifyNewGameStateAvailable(string gameId)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);

        var bots = await gameGrain.GetPlayersAsync();
        var botGrains = bots.Where(x => x.IsBot)
            .Select(bot => new { Grain = clusterClient.GetGrain<IBotGrain>(bot.Id), bot.Mark });

        await Task.WhenAll(
            botGrains.Select(bot => bot.Grain.NewGameStateAvailableAsync(gameId, bot.Mark))
        );
    }
}
