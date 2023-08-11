using TicTacToe.Shared.Models;

namespace TicTacToe.SignalRServer.Models;

public record JoinGameRequest(string GameId);

public record GetCurrentGameStateRequest(string GameId);

public record AddBotRequest(string GameId);

public record AttemptPlayRequest(string GameId, Play Play);
