using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors;

public class GrainProviderReadier : ILifecycleParticipant<IClusterClientLifecycle>
{
    private readonly TaskCompletionSource clusterClientReady = new();
    private readonly ILogger<GrainProviderReadier> logger;

    public Task ClusterClientReady => clusterClientReady.Task;

    public GrainProviderReadier(ILogger<GrainProviderReadier> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Participate(IClusterClientLifecycle observer)
    {
        logger.LogDebug("Participating in lifecycle");
        observer.Subscribe<GrainProviderReadier>(
            ServiceLifecycleStage.Active,
            (_cancellation) =>
            {
                logger.LogDebug("ClusterClient ready");
                clusterClientReady.TrySetResult();
                return Task.CompletedTask;
            }
        );
    }
}
