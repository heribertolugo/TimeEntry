using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using TimeClock.Maui.Services;

namespace TimeClock.Maui.Helpers
{
    internal static class ServiceHelper
    {
       public static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters) where T : class =>
                ActivatorUtilities.CreateInstance<T>(provider, parameters);
        public static TService? GetService<TService>()
        {
            if (Current is null)
                return default(TService);
            return Current.GetService<TService>();
        }

        public static ITimeClockApiAccessService? GetApiService(bool throwIfNotFound = true)
        {
            if (Current is null)
                return null;
            ITimeClockApiAccessService? service = Current.GetRequiredService<ITimeClockApiAccessService>() ?? default(ITimeClockApiAccessService);// ServiceHelper.GetService<ITimeClockApiAccessService>();

            if ((service is null || service == default) && throwIfNotFound)
                throw new ApplicationException(CommonValues.ApiNotLoaded);
            return service;
        }

        public static ILogger<T>? GetLoggerService<T>(bool throwIfNotFound = true)
        {
            ILogger<T>? service = ServiceHelper.GetService<ILogger<T>>();

            if (service is null && throwIfNotFound)
                throw new ApplicationException(string.Format(CommonValues.FatalNotFound, nameof(ILogger)));

            return service;
        }

        public static IPopupService? GetPopoupService(bool throwIfNotFound = true)
        {
            IPopupService? service = Helpers.ServiceHelper.GetService<IPopupService>();

            if (service is null && throwIfNotFound)
                throw new ApplicationException(string.Format(CommonValues.FatalNotFound, nameof(IPopupService)));

            return service;
        }

        public static IServiceProvider? Current =>
#if WINDOWS10_0_17763_0_OR_GREATER
			MauiWinUIApplication.Current?.Services;
#elif ANDROID
            MauiApplication.Current?.Services;
#elif IOS || MACCATALYST
                IPlatformApplication.Current?.Services;
        
#else
			null;
#endif
    }
}
