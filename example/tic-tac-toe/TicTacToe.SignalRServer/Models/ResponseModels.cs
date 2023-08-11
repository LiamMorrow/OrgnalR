using TicTacToe.Shared.Models;

namespace TicTacToe.SignalRServer.Models;

public record ConnectedPlayer(bool IsBot);

public record ConnectedGame(GameState State, Mark Mark, ConnectedPlayer? Opponent);
