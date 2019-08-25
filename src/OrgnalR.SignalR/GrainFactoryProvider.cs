using System;
using Orleans;

namespace OrgnalR.SignalR
{
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
