using TicTacToe.Shared.Models;

namespace TicTacToe.SignalRServer.Models;

public record JoinGameRequest(string GameId, string UserId);

public record AttemptPlayRequest(string GameId, string UserId, Play Play);
