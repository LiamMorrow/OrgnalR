using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrgnalR.Backplane;
using OrgnalR.Backplane.GrainAdaptors;
using OrgnalR.Backplane.GrainInterfaces;
using OrgnalR.Core.Provider;
using OrgnalR.Core.State;
using Orleans;

namespace OrgnalR.SignalR
{
    public static class SiloClientExtensions
    {
        /// <summary>
        /// Configures the Orleans client to use the OrgnalR grain interfaces
        /// </summary>
        /// <param name="builder">The orleans client builder to configure</param>
        /// <returns>The configured orleans client builder</returns>
        public static IClientBuilder UseOrgnalR(this IClientBuilder builder)
        {
            builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IAnonymousMessageGrain).Assembly));
            return builder;
        }

    }
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
            try
            {
                try
                {
                    // Most people will register their client as an IClusterClient.
                    builder.Services.AddSingleton<IGrainFactory>(s => s.GetService<IClusterClient>());
                }
                catch { /* Do nothing, already added */}
                // Will pull the grain factory from the registered services
                builder.Services.AddSingleton<IGrainFactoryProvider, GrainFactoryProvider>();
            }
            catch { /* Do nothing, already added */}
            builder.Services.AddSingleton(typeof(IGroupActorProvider<>), typeof(GroupActorProviderFactory<>));
            builder.Services.AddSingleton(typeof(IUserActorProvider<>), typeof(GroupActorProviderFactory<>));
            builder.Services.AddSingleton(typeof(IMessageObservable<>), typeof(MessageObservableFactory<>));
            builder.Services.AddSingleton(typeof(IMessageObserver<>), typeof(MessageObserverFactory<>));
            builder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(OrgnalRHubLifetimeManagerFactory<>));
            return builder;
        }

        #region Generic Wrappers

        // Below are generic versions of the non generic OrgnalR types.
        // We use these to allow SignalR to get the correct OrgnalR services for the specific hub that requires them.
        // This is important because each service acts on a per hub basis

        public interface IGroupActorProvider<T> : IGroupActorProvider
        {
        }
        public interface IUserActorProvider<T> : IUserActorProvider
        {
        }
        public interface IMessageObservable<T> : IMessageObservable
        {
        }
        public interface IMessageObserver<T> : IMessageObserver
        {
        }
        public class MessageObserverFactory<T> : IMessageObserver<T>
        {
            readonly GrainMessageObserver @delegate;
            public MessageObserverFactory(IGrainFactoryProvider grainFactory)
            {
                @delegate = new GrainMessageObserver(typeof(T).Name, grainFactory.GetGrainFactory());
            }
            public Task SendAddressedMessageAsync(AddressedMessage msg, CancellationToken cancellationToken = default)
            {
                return @delegate.SendAddressedMessageAsync(msg, cancellationToken);
            }

            public Task SendAllMessageAsync(AnonymousMessage allMessage, CancellationToken cancellationToken = default)
            {
                return @delegate.SendAllMessageAsync(allMessage, cancellationToken);
            }
        }
        public class MessageObservableFactory<T> : IMessageObservable<T>
        {
            readonly GrainMessageObservable @delegate;
            public MessageObservableFactory(IGrainFactoryProvider grainFactory)
            {
                @delegate = new GrainMessageObservable(typeof(T).Name, grainFactory.GetGrainFactory());
            }

            public Task<SubscriptionHandle> SubscribeToAllAsync(
                Func<AnonymousMessage, MessageHandle, Task> messageCallback,
                Func<SubscriptionHandle, Task> onSubscriptionEnd,
                MessageHandle since = default,
                CancellationToken cancellationToken = default
            )
            {
                return @delegate.SubscribeToAllAsync(messageCallback, onSubscriptionEnd, since, cancellationToken);
            }

            public Task SubscribeToConnectionAsync(string connectionId,
            Func<AddressedMessage, MessageHandle, Task> messageCallback,
            Func<string, Task> onSubscriptionEnd,
            MessageHandle since = default,
            CancellationToken cancellationToken = default
            )
            {
                return @delegate.SubscribeToConnectionAsync(connectionId, messageCallback, onSubscriptionEnd, since, cancellationToken);
            }

            public Task UnsubscribeFromAllAsync(SubscriptionHandle subscriptionHandle, CancellationToken cancellationToken = default)
            {
                return @delegate.UnsubscribeFromAllAsync(subscriptionHandle, cancellationToken);
            }

            public Task UnsubscribeFromConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
            {
                return @delegate.UnsubscribeFromConnectionAsync(connectionId, cancellationToken);
            }
        }

        public class GroupActorProviderFactory<T> : IGroupActorProvider<T>, IUserActorProvider<T>
        {
            readonly GrainActorProvider @delegate;
            public GroupActorProviderFactory(IGrainFactoryProvider grainFactory)
            {
                @delegate = new GrainActorProvider(typeof(T).Name, grainFactory.GetGrainFactory());
            }
            public IGroupActor GetGroupActor(string groupName)
            {
                return @delegate.GetGroupActor(groupName);
            }

            public IUserActor GetUserActor(string userId)
            {
                return @delegate.GetUserActor(userId);
            }
        }

        public class OrgnalRHubLifetimeManagerFactory<T> : HubLifetimeManager<T>
        where T : Hub
        {
            readonly OrgnalRHubLifetimeManager<T> @delegate;
            public OrgnalRHubLifetimeManagerFactory(IServiceProvider services)
            {
                @delegate = OrgnalRHubLifetimeManager<T>.CreateAsync(
                    services.GetService<IGroupActorProvider<T>>(),
                    services.GetService<IUserActorProvider<T>>(),
                    services.GetService<IMessageObservable<T>>(),
                    services.GetService<IMessageObserver<T>>(),
                    services.GetRequiredService<ILogger<OrgnalRHubLifetimeManager<T>>>()
                ).Result;
            }
            public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
            {
                return @delegate.AddToGroupAsync(connectionId, groupName, cancellationToken);
            }

            public override Task OnConnectedAsync(HubConnectionContext connection)
            {
                return @delegate.OnConnectedAsync(connection);
            }

            public override Task OnDisconnectedAsync(HubConnectionContext connection)
            {
                return @delegate.OnDisconnectedAsync(connection);
            }

            public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
            {
                return @delegate.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
            }

            public override Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendAllAsync(methodName, args, cancellationToken);
            }

            public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
            {
                return @delegate.SendAllExceptAsync(methodName, args, excludedConnectionIds, cancellationToken);
            }

            public override Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendConnectionAsync(connectionId, methodName, args, cancellationToken);
            }

            public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendConnectionsAsync(connectionIds, methodName, args, cancellationToken);
            }

            public override Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendGroupAsync(groupName, methodName, args, cancellationToken);
            }

            public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
            {
                return @delegate.SendGroupExceptAsync(groupName, methodName, args, excludedConnectionIds, cancellationToken);
            }

            public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendGroupsAsync(groupNames, methodName, args, cancellationToken);
            }

            public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendUserAsync(userId, methodName, args, cancellationToken);
            }

            public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                return @delegate.SendUsersAsync(userIds, methodName, args, cancellationToken);
            }
        }
        #endregion
    }

}
