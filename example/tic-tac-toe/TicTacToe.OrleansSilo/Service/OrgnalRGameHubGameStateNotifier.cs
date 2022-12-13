using OrgnalR.Core.Provider;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Interfaces.HubClients;
using TicTacToe.Interfaces.Hubs;

namespace TicTacToe.OrleansSilo.Service;

public class OrgnalRGameHubGameStateNotifier : IGameStateNotifier
{
    private readonly IHubContextProvider hubContextProvider;

    public OrgnalRGameHubGameStateNotifier(IHubContextProvider hubContextProvider)
    {
        this.hubContextProvider = hubContextProvider;
    }

    public Task NotifyNewGameStateAvailable(string gameId)
    {
        var clientsInGroup = hubContextProvider
            .GetHubContext<IGameHub, IGameHubClient>()
            .Clients.Group(gameId);

        return clientsInGroup.NewGameStateAvailable(gameId);
    }
}
