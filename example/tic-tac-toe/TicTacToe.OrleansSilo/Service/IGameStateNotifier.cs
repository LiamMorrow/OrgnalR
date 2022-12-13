namespace TicTacToe.OrleansSilo.Service;

public interface IGameStateNotifier
{
    public Task NotifyNewGameStateAvailable(string gameId);
}
