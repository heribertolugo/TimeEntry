using Microsoft.EntityFrameworkCore;
using TimeClock.Api.Security;
using TimeClock.Data;

namespace TimeClock.Api.Helpers;

public static class CommonValues
{
    internal static readonly string PublicKeyNameFormatString = "{0}_PublicEncryptionKey";
    internal static readonly string PrivateKeyNameFormatString = "{0}_PrivateEncryptionKey";
    /// <summary>
    /// Collection of JobStep JDE ID's which require user to select JobStep and/or JobType
    /// </summary>
    internal static readonly string[] JobStepsRequireSelection = ["099"];

    /// <summary>
    /// Creates a JobType and returns it if found in database. 
    /// The following fields are needed: User.UnionCode, User.DefaultJobType.JdeId, User.Id, Location.DivisionCode
    /// </summary>
    /// <param name="locationDivisionCode"></param>
    /// <param name="unionCode"></param>
    /// <param name="defaultJobTypeId"></param>
    /// <param name="data"></param>
    /// <param name="secretsProvider"></param>
    /// <returns></returns>
    internal static async Task<Guid?> GetJobTypeId(string? locationDivisionCode, string? unionCode, Guid? defaultJobTypeId, IDataRepositoryFactory data, SecretsProvider secretsProvider)
    {
        // job type -> project for all users => [location.division] + [union code (last 2)]
        KeyValuePair<string, string>[] jobCodeMap = secretsProvider.GetLocationDivisionsToJobCodes();
        string? jobCode = jobCodeMap.Cast<KeyValuePair<string,string>?>().FirstOrDefault(m => m?.Key == locationDivisionCode, null)?.Value;

        if (jobCode is null || unionCode is null)
        {
            if (defaultJobTypeId.HasValue)
            {
                return (await data.GetJobTypesRepository().GetAsync(defaultJobTypeId ?? default, false))?.Id;
            }
            return null;
        }

        string jobTypeCreated = $"U{jobCode}{string.Join("", unionCode.TakeLast(3))}";

        var candidate = (await data.GetJobTypesRepository().GetAsync(j => j.JdeId.Trim() == jobTypeCreated))
            .FirstOrDefault();

        if (candidate is null)
        {
            var pattern = $"_{jobCode}_{string.Join("", unionCode.TakeLast(2))}";
            return (await data.GetJobTypesRepository()
                .GetAsync(j => EF.Functions.Like(j.JdeId.Trim(), pattern)))
                .FirstOrDefault()?.Id;                
        }
        return candidate?.Id;
    }
}
