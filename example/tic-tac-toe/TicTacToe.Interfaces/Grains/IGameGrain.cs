using Orleans;
using TicTacToe.Shared.Models;

namespace TicTacToe.Interfaces.Grains;

public interface IGameGrain : IGrainWithStringKey
{
    Task<GameState> GetGameStateAsync();
    Task AttemptPlayAsync(string userId, Play play);
    Task<Mark> JoinGameAsync(string gameId, string userId);
}
