using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OrgnalR.Core.Provider
{
    public class AllMessage
    {
        public ISet<string> Excluding { get; }
        public HubInvocationMessage Payload { get; }
        public AllMessage(ISet<string> excluding, HubInvocationMessage messagePayload)
        {
            Excluding = excluding;
            Payload = messagePayload;
        }
    }

}