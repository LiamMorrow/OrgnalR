using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using TicTacToe.Engine.Bot;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Shared.Models;

namespace TicTacToe.OrleansSilo.GrainImplementations;

public class BotGrain : IBotGrain, IGrainBase
{
    private readonly IClusterClient clusterClient;
    private readonly ILogger<BotGrain> logger;
    private readonly IBot bot = new RandomMoveBot(new Random());

    public IGrainContext GrainContext { get; }

    public BotGrain(
        IGrainContext grainContext,
        IClusterClient clusterClient,
        ILogger<BotGrain> logger
    )
    {
        GrainContext = grainContext;
        this.clusterClient = clusterClient;
        this.logger = logger;
    }

    public async Task NewGameStateAvailableAsync(string gameId, Mark botMark)
    {
        var gameGrain = clusterClient.GetGrain<IGameGrain>(gameId);
        var gameState = await gameGrain.GetGameStateAsync();
        if (!gameState.GameOver && gameState.Turn == botMark)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            {
                var move = bot.GetNextPlay(gameState);
                try
                {
                    await gameGrain.AttemptPlayAsync(this.GetPrimaryKeyString(), move);
                }
                catch (InvalidOperationException e)
                {
                    logger.LogError(e, "Bot tried to make an illegal move {move}", move);
                }
            }
        }
    }
}
