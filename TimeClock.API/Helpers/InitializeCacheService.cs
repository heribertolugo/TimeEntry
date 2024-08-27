using Microsoft.Extensions.Caching.Memory;
using System;

namespace TimeClock.Api.Helpers
{
    public class InitializeCacheService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public InitializeCacheService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IServiceScopeFactory? serviceScopeFactory = this._serviceProvider.GetService<IServiceScopeFactory>();

            if (serviceScopeFactory == null) return;

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                if (scope.ServiceProvider.GetService(typeof(ITimeClockCacheDataProvider)) is not ITimeClockCacheDataProvider cache) return;

#warning should use cancellation tokens
                var d = await cache.GetDepartments();
                var l = await cache.GetLocations();
                var v = await cache.GetDevices();
                var c = await cache.GetUserClaims();
                var u = await cache.GetUsers();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
