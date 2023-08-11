namespace TicTacToe.OrleansSilo.Service;

public interface IGameStateNotifier
{
    public void NotifyNewGameStateAvailable(string gameId);
}
