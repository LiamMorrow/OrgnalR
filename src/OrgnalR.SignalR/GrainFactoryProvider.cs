using System;
using Orleans;

namespace OrgnalR.SignalR
{
    /// <summary>
    /// Provides the grain factory to the OrgnalR backplane.
    /// See <see cref="GrainFactoryProvider"/> for a simple implementation.
    /// </summary>
    public interface IGrainFactoryProvider
    {
        IGrainFactory GetGrainFactory();
    }
    public class GrainFactoryProvider : IGrainFactoryProvider
    {
        private readonly IGrainFactory factory;

        public GrainFactoryProvider(IGrainFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IGrainFactory GetGrainFactory() => factory;
    }
}
