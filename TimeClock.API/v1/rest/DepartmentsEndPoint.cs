using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;

namespace TimeClock.Api.v1.rest
{
    public class DepartmentsEndPoint
    {
        public static Delegate GetDepartmentsDelegate = GetDepartments;

        [AllowAnonymous]
        private static async Task<Results<Ok<IEnumerable<DepartmentDto>>, BadRequest>> GetDepartments(HttpContext context, IDataRepositoryFactory data, ILogger<DepartmentsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache
            , string deviceId)
        {
            //string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
            Guid deviceGuid;

            if (!Guid.TryParse(deviceId, out deviceGuid)) //token == null || 
                return TypedResults.BadRequest();

            var departments = await cache.GetDepartments();

            return TypedResults.Ok(departments.OrderBy(d => d.Name).Adapt<IEnumerable<DepartmentDto>>());
        }
    }
}
