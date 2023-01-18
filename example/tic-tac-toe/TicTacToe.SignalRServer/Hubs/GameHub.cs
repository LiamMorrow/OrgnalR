using Microsoft.AspNetCore.SignalR;
using TicTacToe.Interfaces.HubClients;
using TicTacToe.Interfaces.Hubs;
using TicTacToe.Shared.Models;
using TicTacToe.SignalRServer.Models;
using TicTacToe.SignalRServer.Services;

namespace TicTacToe.SignalRServer.Hubs;

public class GameHub : Hub<IGameHubClient>, IGameHub
{
    private readonly GameService gameService;

    public GameHub(GameService gameService)
    {
        this.gameService = gameService;
    }

    public Task<ConnectedGame> GetCurrentGameState(GetCurrentGameStateRequest request)
    {
        return gameService.GetGameStateAsync(request.GameId, Context.ConnectionId);
    }

    public async Task<Mark> JoinGame(JoinGameRequest request)
    {
        var mark = await gameService.JoinGameAsync(request.GameId, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        return mark;
    }

    public async Task AddBot(AddBotRequest request)
    {
        await gameService.AddBotAsync(request.GameId);
    }

    public async Task AttemptPlay(AttemptPlayRequest request)
    {
        await gameService.AttemptPlayAsync(request.GameId, Context.ConnectionId, request.Play);
    }
}
