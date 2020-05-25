using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OrgnalR.Core.Provider
{
    public class AnonymousMessage
    {
        public ISet<string> Excluding { get; }
        public MethodMessage Payload { get; }

        public AnonymousMessage(ISet<string> excluding, MethodMessage payload)
        {
            Excluding = excluding;
            Payload = payload;
        }
    }

}
