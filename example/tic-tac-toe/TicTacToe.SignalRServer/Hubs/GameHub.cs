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

    public Task<GameState> GetCurrentGameState(string gameId)
    {
        return gameService.GetGameStateAsync(gameId);
    }

    public async Task<Mark> JoinGame(JoinGameRequest request)
    {
        var mark = await gameService.JoinGameAsync(request.GameId, request.UserId);
        await Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        return mark;
    }

    public async Task AttemptPlay(AttemptPlayRequest request)
    {
        await gameService.AttemptPlayAsync(request.GameId, request.UserId, request.Play);
    }
}
