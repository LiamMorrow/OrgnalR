using Orleans.Runtime;
using TicTacToe.Engine.Bot;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Shared.Models;

namespace TicTacToe.OrleansSilo.GrainImplementations;

public class BotGrain : IBotGrain, IGrainBase
{
    private readonly IClusterClient clusterClient;
    private readonly IBot bot = new RandomMoveBot(new Random());

    public IGrainContext GrainContext { get; }

    public BotGrain(IGrainContext grainContext, IClusterClient clusterClient)
    {
        GrainContext = grainContext;
        this.clusterClient = clusterClient;
    }

    public async Task NewGameStateAvailableAsync(string gameId, Mark botMark)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);
        var gameState = await gameGrain.GetGameStateAsync();
        if (!gameState.GameOver && gameState.Turn == botMark)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            var move = bot.GetNextPlay(gameState);
            await gameGrain.AttemptPlayAsync(this.GetPrimaryKeyString(), move);
        }
    }
}
