namespace TimeClock.Maui.Helpers;
internal class PermissionsManager
{
    public static bool IsLocationRequired() => DeviceInfo.Current.IsMobile() && !Settings.IsPublicDevice;
    public static async Task<Location?> GetOrRequestLocation(bool isRequired = false, bool getCached = false)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
            return await GetLocation(getCached);

        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>() && isRequired)
            await Shell.Current.DisplayAlert("TimeCock Needs Permission", "Location information is required for this action.", "Ok");

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted && isRequired)
            await Shell.Current.DisplayAlert("TimeCock Needs Permission", "Location information is required for this action.", "ok");

        return await GetLocation(getCached);
    }

    public static async Task<Location?> GetLocation(bool getCached = false)
    {
        try
        {
            if (getCached)
                return await Geolocation.Default.GetLastKnownLocationAsync();
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromMilliseconds(CommonValues.ApiCancellationTokenLimit));
#if IOS
request.RequestFullAccuracy = true;
#endif
            Location? location = await Geolocation.Default.GetLocationAsync(request, new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit).Token);
            return (location?.IsFromMockProvider ?? true) ? null : location;
        }
        catch (Exception) { return null; }
    }
}
