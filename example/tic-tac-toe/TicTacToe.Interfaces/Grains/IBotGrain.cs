using Orleans;
using TicTacToe.Shared.Models;

namespace TicTacToe.Interfaces.Grains;

public interface IBotGrain : IGrainWithStringKey
{
    Task NewGameStateAvailableAsync(string gameId, Mark botMark);
}
