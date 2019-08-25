using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OrgnalR.Core.Provider
{
    public class AnonymousMessage
    {
        public ISet<string> Excluding { get; }
        public HubInvocationMessage Payload { get; }
        public AnonymousMessage(ISet<string> excluding, HubInvocationMessage messagePayload)
        {
            Excluding = excluding;
            Payload = messagePayload;
        }
    }

}
