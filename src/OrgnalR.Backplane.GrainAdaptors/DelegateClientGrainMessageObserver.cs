using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Protocol;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.Backplane.GrainAdaptors
{
    public class DelegateClientGrainMessageObserver : IClientMessageObserver
    {
        private readonly string connectionId;
        private readonly Func<AddressedMessage, Task> messageCallback;
        private readonly Func<string, Task> onSubscriptionEnded;

        public DelegateClientGrainMessageObserver(string connectionId, Func<AddressedMessage, Task> messageCallback, Func<string, Task> onSubscriptionEnded)
        {
            this.connectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));
            this.messageCallback = messageCallback ?? throw new ArgumentNullException(nameof(messageCallback));
            this.onSubscriptionEnded = onSubscriptionEnded ?? throw new ArgumentNullException(nameof(onSubscriptionEnded));
        }

        public void ReceiveMessage(HubInvocationMessage message)
        {
            messageCallback(new AddressedMessage(connectionId, message)).Ignore();
        }

        public void SubscriptionEnded()
        {
            onSubscriptionEnded(connectionId).Ignore();
        }
    }
}