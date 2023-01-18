using TicTacToe.Shared.Models;

namespace TicTacToe.SignalRServer.Models;

public record JoinGameRequest(string GameId, string UserId);

public record GetCurrentGameStateRequest(string GameId, string UserId);

public record AddBotRequest(string GameId);

public record AttemptPlayRequest(string GameId, string UserId, Play Play);
