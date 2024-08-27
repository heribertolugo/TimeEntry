using System.DirectoryServices;
using System.Runtime.InteropServices;
using TimeClock.Data;
using TimeClock.Data.Models;

namespace TimeClock.Api.Helpers;

public class ActiveDirectoryHelper
{
    public static bool AuthenticateWithEntry(string username, string password, IDataRepositoryFactory data, ILogger? logger = null)
    {
        bool success = false;
        using (DirectoryEntry entry = new() { Username = username, Password = password })
        {
            DirectorySearcher searcher = new(entry)
            {
                Filter = "(objectclass=user)"
            };

            try
            {
                searcher.FindOne();
                success = true;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == -2147023570)
                {
                    if (logger is not null)
                    {
                        logger.LogAudit<User>($"{nameof(ActiveDirectoryHelper.AuthenticateWithEntry)} Login or password is incorrect. username: {username} password: {password}",
                        EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
                        success = false;
                    }
                }
                else
                {
#warning DELETE - THIS IS ONLY GOOD FOR WHEN DEBUGGING ON LOCAL
                    if (ex.ErrorCode == -2147023541)
                    {
                        // https://learn.microsoft.com/en-us/uwp/api/windows.security.credentials.ui.userconsentverifier?view=winrt-22621
                        var consentResult = Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("Please verify").GetAwaiter().GetResult();
                        if (consentResult == Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified)
                            return true;
                    }
                    if (logger is not null)
                    {
                        logger.LogAudit<User>($"{nameof(ActiveDirectoryHelper.AuthenticateWithEntry)} ComException. message: {ex.Message}",
                            EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
                        success = false;
                    }
                }
            }
        }
        return success;
    }
}
