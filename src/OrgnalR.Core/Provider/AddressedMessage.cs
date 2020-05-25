using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace OrgnalR.Core.Provider
{
    public class AddressedMessage
    {
        public string ConnectionId { get; }
        public MethodMessage Payload { get; }
        public AddressedMessage(string connectionId, MethodMessage payload)
        {
            ConnectionId = connectionId;
            Payload = payload;
        }
    }

}
