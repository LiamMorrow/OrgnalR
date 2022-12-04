using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrgnalR.Backplane.GrainAdaptors;
using OrgnalR.Core.Provider;
using Orleans;

namespace OrgnalR.SignalR
{
    public static class SignalRExtensions
    {
        /// <summary>
        /// Configures SignalR to use the OrgnalR backplane.  Requires that an <see cref="IClusterClient"/> is registered.
        /// If at runtime, your application cannot resolve an IClusterClient, ensure you register one.
        /// Alternatively, before calling this method, register an <see cref="IGrainFactoryProvider"/>.
        /// </summary>
        /// <param name="builder">The SignalR build to configure</param>
        /// <returns>The same same builder, configured to use OrgnalR</returns>
        public static ISignalRBuilder UseOrgnalR(this ISignalRBuilder builder)
        {
            // Will pull the grain factory from the registered services
            builder.Services.AddSingleton<IGrainFactoryProvider, GrainFactoryProvider>();
            builder.Services.AddSingleton<IMessageArgsSerializer, OrleansMessageArgsSerializer>();
            builder.Services.AddSingleton<IActorProviderFactory, GrainActorProviderFactory>();
            builder.Services.AddSingleton<IHubContextProvider, HubContextProvider>();
            builder.Services.AddSingleton(
                typeof(IMessageObservable<>),
                typeof(MessageObservableFactory<>)
            );
            builder.Services.AddSingleton(
                typeof(IMessageObserver<>),
                typeof(MessageObserverFactory<>)
            );
            builder.Services.AddSingleton(
                typeof(HubLifetimeManager<>),
                typeof(OrgnalRHubLifetimeManagerFactory<>)
            );
            builder.Services.AddSingleton<GrainProviderReadier>();

            builder.Services.AddSingleton<ILifecycleParticipant<IClusterClientLifecycle>>(
                (svc) => svc.GetRequiredService<GrainProviderReadier>()
            );
            return builder;
        }

        #region Generic Wrappers

        // Below are generic versions of the non generic OrgnalR types.
        // We use these to allow SignalR to get the correct OrgnalR services for the specific hub that requires them.
        // This is important because each service acts on a per hub basis

        public interface IMessageObservable<T> : IMessageObservable { }

        public interface IMessageObserver<T> : IMessageObserver { }

        public class MessageObserverFactory<T> : IMessageObserver<T>
        {
            readonly GrainMessageObserver @delegate;

            public MessageObserverFactory(IGrainFactoryProvider grainFactory)
            {
                @delegate = new GrainMessageObserver(
                    typeof(T).Name,
                    grainFactory.GetGrainFactory()
                );
            }

            public Task SendAddressedMessageAsync(
                AddressedMessage msg,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.SendAddressedMessageAsync(msg, cancellationToken);
            }

            public Task SendAllMessageAsync(
                AnonymousMessage allMessage,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.SendAllMessageAsync(allMessage, cancellationToken);
            }
        }

        public class MessageObservableFactory<T> : IMessageObservable<T>
        {
            readonly GrainMessageObservable @delegate;

            public MessageObservableFactory(
                IGrainFactoryProvider grainFactory,
                GrainProviderReadier grainProviderReadier
            )
            {
                @delegate = new GrainMessageObservable(
                    typeof(T).Name,
                    grainFactory.GetGrainFactory(),
                    grainProviderReadier
                );
            }

            public Task<SubscriptionHandle> SubscribeToAllAsync(
                Func<AnonymousMessage, MessageHandle, Task> messageCallback,
                Func<SubscriptionHandle, Task> onSubscriptionEnd,
                MessageHandle since = default,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.SubscribeToAllAsync(
                    messageCallback,
                    onSubscriptionEnd,
                    since,
                    cancellationToken
                );
            }

            public Task SubscribeToConnectionAsync(
                string connectionId,
                Func<AddressedMessage, MessageHandle, Task> messageCallback,
                Func<string, Task> onSubscriptionEnd,
                MessageHandle since = default,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.SubscribeToConnectionAsync(
                    connectionId,
                    messageCallback,
                    onSubscriptionEnd,
                    since,
                    cancellationToken
                );
            }

            public Task UnsubscribeFromAllAsync(
                SubscriptionHandle subscriptionHandle,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.UnsubscribeFromAllAsync(subscriptionHandle, cancellationToken);
            }

            public Task UnsubscribeFromConnectionAsync(
                string connectionId,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.UnsubscribeFromConnectionAsync(connectionId, cancellationToken);
            }
        }

        public class OrgnalRHubLifetimeManagerFactory<T> : HubLifetimeManager<T> where T : Hub
        {
            readonly Task<OrgnalRHubLifetimeManager<T>> @delegate;

            public OrgnalRHubLifetimeManagerFactory(IServiceProvider services)
            {
                @delegate = OrgnalRHubLifetimeManager<T>.CreateAsync(
                    services.GetRequiredService<IActorProviderFactory>(),
                    services.GetRequiredService<IMessageObservable<T>>(),
                    services.GetRequiredService<IMessageObserver<T>>(),
                    services.GetRequiredService<IMessageArgsSerializer>(),
                    services.GetRequiredService<ILogger<OrgnalRHubLifetimeManager<T>>>()
                );
            }

            public override async Task AddToGroupAsync(
                string connectionId,
                string groupName,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).AddToGroupAsync(connectionId, groupName, cancellationToken);
            }

            public override async Task OnConnectedAsync(HubConnectionContext connection)
            {
                try
                {
                    await (await @delegate).OnConnectedAsync(connection);
                }
                catch (Exception error)
                {
                    Console.Error.WriteLine(error);
                    throw;
                }
            }

            public override async Task OnDisconnectedAsync(HubConnectionContext connection)
            {
                await (await @delegate).OnDisconnectedAsync(connection);
            }

            public override async Task RemoveFromGroupAsync(
                string connectionId,
                string groupName,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).RemoveFromGroupAsync(
                    connectionId,
                    groupName,
                    cancellationToken
                );
            }

            public override async Task SendAllAsync(
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendAllAsync(methodName, args, cancellationToken);
            }

            public override async Task SendAllExceptAsync(
                string methodName,
                object?[] args,
                IReadOnlyList<string> excludedConnectionIds,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendAllExceptAsync(
                    methodName,
                    args,
                    excludedConnectionIds,
                    cancellationToken
                );
            }

            public override async Task SendConnectionAsync(
                string connectionId,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendConnectionAsync(
                    connectionId,
                    methodName,
                    args,
                    cancellationToken
                );
            }

            public override async Task SendConnectionsAsync(
                IReadOnlyList<string> connectionIds,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendConnectionsAsync(
                    connectionIds,
                    methodName,
                    args,
                    cancellationToken
                );
            }

            public override async Task SendGroupAsync(
                string groupName,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendGroupAsync(
                    groupName,
                    methodName,
                    args,
                    cancellationToken
                );
            }

            public override async Task SendGroupExceptAsync(
                string groupName,
                string methodName,
                object?[] args,
                IReadOnlyList<string> excludedConnectionIds,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendGroupExceptAsync(
                    groupName,
                    methodName,
                    args,
                    excludedConnectionIds,
                    cancellationToken
                );
            }

            public override async Task SendGroupsAsync(
                IReadOnlyList<string> groupNames,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendGroupsAsync(
                    groupNames,
                    methodName,
                    args,
                    cancellationToken
                );
            }

            public override async Task SendUserAsync(
                string userId,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendUserAsync(userId, methodName, args, cancellationToken);
            }

            public override async Task SendUsersAsync(
                IReadOnlyList<string> userIds,
                string methodName,
                object?[] args,
                CancellationToken cancellationToken = default
            )
            {
                await (await @delegate).SendUsersAsync(
                    userIds,
                    methodName,
                    args,
                    cancellationToken
                );
            }
        }
        #endregion
    }
}
