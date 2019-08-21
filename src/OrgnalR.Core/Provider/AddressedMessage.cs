using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OrgnalR.Core.Provider
{
    public class AddressedMessage
    {
        public string ConnectionId { get; }
        public HubInvocationMessage Payload { get; }
        public AddressedMessage(string connectionId, HubInvocationMessage messagePayload)
        {
            ConnectionId = connectionId;
            Payload = messagePayload;
        }
    }

}