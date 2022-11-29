using System.Collections.Generic;
using Orleans;

namespace OrgnalR.Core.Provider
{
    [GenerateSerializer]
    public class AnonymousMessage
    {
        [Id(0)]
        public ISet<string> Excluding { get; }
        [Id(1)]
        public MethodMessage Payload { get; }

        public AnonymousMessage(ISet<string> excluding, MethodMessage payload)
        {
            Excluding = excluding;
            Payload = payload;
        }
    }

}
