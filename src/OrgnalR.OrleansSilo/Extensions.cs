using System;
using Orleans.Hosting;
using OrgnalR.Backplane.GrainImplementations;
using Orleans;

namespace OrgnalR.Silo
{
    public static class Extensions
    {
        /// <summary>
        /// Adds the OrgnalR grains to the builder, and also automatically registers memory grain storage for group and user lists.
        /// This is useful for local development, however it is recommended that you add a persistent storage for:
        /// <see cref="Constants.GROUP_STORAGE_PROVIDER"/>, and <see cref="Constants.USER_STORAGE_PROVIDER"/>
        /// Then you may use <see cref="AddOrgnalR<T>(T builder)"/> to add orgnalr using the storage providers of your choice
        /// </summary>
        /// <param name="builder">The builder to configure</param>
        /// <returns>The silo builder, configured with memory storage and grains for the OrgnalR backplane</returns>
        public static ISiloHostBuilder AddOrgnalRWithMemoryGrainStorage(this ISiloHostBuilder builder)
        {
            try
            {
                builder.AddMemoryGrainStorage(Constants.GROUP_STORAGE_PROVIDER);
            }
            catch { /* Do nothing, already added  */}
            try
            {
                builder.AddMemoryGrainStorage(Constants.USER_STORAGE_PROVIDER);
            }
            catch { /* Do nothing, already added  */}

            return builder.AddOrgnalR();
        }

        /// <summary>
        /// Adds the OrgnalR grains to the builder. This method is recommended for production use.
        /// You must configure storage providers for:
        /// <see cref="Constants.GROUP_STORAGE_PROVIDER"/>, and <see cref="Constants.USER_STORAGE_PROVIDER"/>
        /// Alternatively, for local development, use: <see cref="AddOrgnalRWithMemoryGrainStorage<T>(T builder)"/>
        /// </summary>
        /// <param name="builder">The builder to configure</param>
        /// <returns>The silo builder, configured with grains for the OrgnalR backplane</returns>
        public static ISiloHostBuilder AddOrgnalR(this ISiloHostBuilder builder)
        {
            builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(AnonymousMessageGrain).Assembly).WithReferences());
            return builder;
        }
    }
}
