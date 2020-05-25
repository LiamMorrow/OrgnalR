using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class DelegateClientMessageObserver : IClientMessageObserver
    {
        private readonly string connectionId;
        private readonly Func<AddressedMessage, MessageHandle, Task> messageCallback;
        private readonly Func<string, Task> onSubscriptionEnded;

        public DelegateClientMessageObserver(string connectionId, Func<AddressedMessage, MessageHandle, Task> messageCallback, Func<string, Task> onSubscriptionEnded)
        {
            this.connectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));
            this.messageCallback = messageCallback ?? throw new ArgumentNullException(nameof(messageCallback));
            this.onSubscriptionEnded = onSubscriptionEnded ?? throw new ArgumentNullException(nameof(onSubscriptionEnded));
        }

        public void ReceiveMessage(MethodMessage message, MessageHandle handle)
        {
            messageCallback(new AddressedMessage(connectionId, message), handle).Ignore();
        }

        public void SubscriptionEnded()
        {
            onSubscriptionEnded(connectionId).Ignore();
        }
    }
}
