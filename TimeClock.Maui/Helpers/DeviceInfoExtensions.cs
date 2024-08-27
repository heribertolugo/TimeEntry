namespace TimeClock.Maui.Helpers;
internal static class DeviceInfoExtensions
{
    public static bool IsMobile(this IDeviceInfo deviceInfo)
    {
        return deviceInfo.Idiom != DeviceIdiom.Desktop && deviceInfo.Idiom != DeviceIdiom.TV && 
            (deviceInfo.Platform == DevicePlatform.Android || deviceInfo.Platform == DevicePlatform.iOS);
    }
}
