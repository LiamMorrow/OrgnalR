namespace TicTacToe.Interfaces.HubClients;

public interface IGameHubClient
{
    Task NewGameStateAvailable(string gameId);
}
