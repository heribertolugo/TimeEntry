using Mapster;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TimeClock.Core;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;
using TimeClock.Core.Models.EntityDtos;
using Location = TimeClock.Data.Models.Location;
using ThisCacheProvider = TimeClock.Api.Helpers.TimeClockCacheDataProvider;

namespace TimeClock.Api.Helpers
{
    internal interface ITimeClockCacheDataProvider
    {
        void AddDeviceId(Guid deviceId);
        void AddPendingDevice(UnregisteredDeviceDto deviceId);
        Task RemovePendingDevice(Guid deviceId);
        void AddToCache<T>(string key, T value, SemaphoreSlim semaphore);
        void AddToCachedCollection<T>(string key, T value, SemaphoreSlim semaphore);
        Task<IEnumerable<T>> GetCachedCollection<T>(string key, SemaphoreSlim semaphore);
        ValueTask<IEnumerable<DepartmentDto>> GetDepartments(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Department, object>>[] includes);
        ValueTask<IDictionary<Guid, DeviceDto>> GetDevices(bool refresh = false);
        ValueTask<DeviceDto?> GetDevice(Guid id);
        ValueTask<IEnumerable<UnregisteredDeviceDto>> GetPendingDevices(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<UnregisteredDevice, object>>[] includes);
        ValueTask<IEnumerable<LocationDto>> GetLocations(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Location, object>>[] includes);
        ValueTask<IEnumerable<UserClaimDto>> GetUserClaims(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<UserClaim, object>>[] includes);
        ValueTask<IEnumerable<UserDto>> GetUsers(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<User, object>>[] includes);
        ValueTask<IEnumerable<BarcodeDto>> GetBarcodes(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Barcode, object>>[] includes);
        ValueTask<IEnumerable<JobTypeDto>> GetJobTypes(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<JobType, object>>[] includes);
        ValueTask<IEnumerable<JobStepDto>> GetJobSteps(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<JobStep, object>>[] includes);
        Task RefeshAll(bool force = false);
        Task<IEnumerable<T2>> GetOrSetCachedDbCollection<T1, T2>(string key, SemaphoreSlim semaphore, IDataRepository<T1> data, int size = default, CancellationToken cancellationToken = default, params Expression<Func<T1, object>>[] includes) where T1 : class, IEntityModel;
    }

    internal sealed class TimeClockCacheDataProvider : ITimeClockCacheDataProvider
    {
        #region Semaphores
        public static readonly SemaphoreSlim DeviceIdsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim PendingDeviceIdsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim LocationsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim DepartmentsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim UserClaimsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim UsersSemaphore = new(1, 1);
        public static readonly SemaphoreSlim BarcodesSemaphore = new(1, 1);
        public static readonly SemaphoreSlim DepartmentsToLocationsSemaphore = new(1, 1);
        public static readonly SemaphoreSlim JobTypesSemaphore = new(1, 1);
        public static readonly SemaphoreSlim JobStepsSemaphore = new(1, 1);
        #endregion Semaphores

        #region Private Members/Properties
        public static readonly string PendingDeviceIdsKey = "$PendingDeviceIds";
        public static readonly string DeviceIdsKey = "$DeviceIds";
        public static readonly string LocationsKey = "$Locations";
        public static readonly string DepartmentsKey = "$Departments";
        public static readonly string UsersKey = "$Users";
        public static readonly string UserClaimsKey = "$UserClaims";
        public static readonly string BarcodesKey = "$Barcodes";
        public static readonly string DepartmentsToLocationsKey = "$DepartmentsToLocations";
        public static readonly string JobTypesKey = "$JobTypes";
        public static readonly string JobStepsKey = "$JobSteps";

        private IDataRepositoryFactory DataRepo { get; init; }
        private IMemoryCache Cache { get; init; }
        #endregion Private Members/Properties

        public TimeClockCacheDataProvider(IMemoryCache memoryCache, IDataRepositoryFactory data)
        {
            this.Cache = memoryCache;
            this.DataRepo = data;
        }

        public void AddPendingDevice(UnregisteredDeviceDto device)
        {
            this.AddToCachedCollection(ThisCacheProvider.PendingDeviceIdsKey, device, ThisCacheProvider.PendingDeviceIdsSemaphore);
        }

        public async Task RemovePendingDevice(Guid deviceId)
        {
            await ThisCacheProvider.PendingDeviceIdsSemaphore.WaitAsync();
            var ids = await this.GetPendingDevices();
            ids = ids.Where(d => d.Id != deviceId);

            this.Cache.Set(ThisCacheProvider.PendingDeviceIdsKey, ids);
            ThisCacheProvider.PendingDeviceIdsSemaphore.Release();
        }

        public async ValueTask<IEnumerable<UnregisteredDeviceDto>> GetPendingDevices(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<UnregisteredDevice, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.PendingDeviceIdsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.PendingDeviceIdsKey);
                ThisCacheProvider.PendingDeviceIdsSemaphore.Release();
            }
            //return await this.GetCachedCollection<UnregisteredDeviceDto>(ThisCacheProvider.PendingDeviceIdsKey, ThisCacheProvider.PendingDeviceIdsSemaphore);
            return await this.GetOrSetCachedDbCollection<UnregisteredDevice, UnregisteredDeviceDto>(ThisCacheProvider.PendingDeviceIdsKey, ThisCacheProvider.PendingDeviceIdsSemaphore, this.DataRepo.GetUnregisteredDevicesRepository()
                , cancellationToken: cancellationToken, includes: includes);
        }

        public void AddDeviceId(Guid deviceId)
        {
            this.AddToCachedCollection(ThisCacheProvider.DeviceIdsKey, deviceId, ThisCacheProvider.DeviceIdsSemaphore);
        }

        public async ValueTask<DeviceDto?> GetDevice(Guid id)
        {
            bool isNew = false;
            DeviceDto? device;
            IDictionary<Guid, DeviceDto> data;
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.EndOfDay(),
                SlidingExpiration = TimeSpan.FromHours(8),
                Size = 500,
            };

            if (!(data = await this.GetOrSetDeviceIds()).TryGetValue(id, out device)) 
            {
                device = this.DataRepo.GetDevicesRepository().Get(id)?.Adapt<DeviceDto>();
                isNew = true;
            }
            await ThisCacheProvider.DeviceIdsSemaphore.WaitAsync();

            if (isNew && device is not null)
            {
                data.Add(id, device);
                this.Cache.Remove(ThisCacheProvider.DeviceIdsKey);
                this.Cache.Set(ThisCacheProvider.DeviceIdsKey, data, cacheOptions);
            }
            
            ThisCacheProvider.DeviceIdsSemaphore.Release();

            return device;
        }

        public async ValueTask<IDictionary<Guid, DeviceDto>> GetDevices(bool refresh = false)
        {
            if (refresh)
            {
                await ThisCacheProvider.DeviceIdsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.DeviceIdsKey);
                ThisCacheProvider.DeviceIdsSemaphore.Release();
            }
            return await this.GetOrSetDeviceIds();
        }
        private async Task<IDictionary<Guid, DeviceDto>> GetOrSetDeviceIds()
        {
            Dictionary<Guid, DeviceDto>? items;

            try
            {
                // we use thread lock because GetOrCreate is not thread safe
                // see https://github.com/dotnet/runtime/issues/36499
                await ThisCacheProvider.DeviceIdsSemaphore.WaitAsync();

                items = await this.Cache.GetOrCreateAsync(ThisCacheProvider.DeviceIdsKey, async c =>
                {
                    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.EndOfDay(),
                        SlidingExpiration = TimeSpan.FromHours(8),
                        Size = 500,
                    };

                    return (await this.DataRepo.GetDevicesRepository().GetActiveAsync()).ToDictionary(d => d.Id, d => d.Adapt<DeviceDto>());
                });
            }
            catch
            {
                throw;
            }
            finally
            {
                ThisCacheProvider.DeviceIdsSemaphore.Release();
            }

            return items ?? new Dictionary<Guid, DeviceDto>();
        }

        public async ValueTask<IEnumerable<LocationDto>> GetLocations(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Location, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.LocationsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.LocationsKey);
                ThisCacheProvider.LocationsSemaphore.Release();
            }
            return await this.GetOrSetCachedDbCollection<Location, LocationDto>(ThisCacheProvider.LocationsKey, ThisCacheProvider.LocationsSemaphore, this.DataRepo.GetLocationsRepository()
                , cancellationToken: cancellationToken, includes: includes);
        }

        public async ValueTask<IEnumerable<DepartmentDto>> GetDepartments(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Department, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.DepartmentsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.DepartmentsKey);
                ThisCacheProvider.DepartmentsSemaphore.Release();
            }
            return await this.GetOrSetCachedDbCollection<Department,DepartmentDto>(ThisCacheProvider.DepartmentsKey, ThisCacheProvider.DepartmentsSemaphore, this.DataRepo.GetDepartmentsRepository()
                , cancellationToken: cancellationToken, includes: includes);
        }

        public async ValueTask<IEnumerable<UserClaimDto>> GetUserClaims(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<UserClaim, object>>[] includes)
        {
            if (refresh) 
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.UserClaimsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.UserClaimsKey);
                ThisCacheProvider.UserClaimsSemaphore.Release();
            }
            var claims = await this.GetCachedCollection<UserClaimDto>(ThisCacheProvider.UserClaimsKey, ThisCacheProvider.UserClaimsSemaphore);

            if (claims is null || !claims.Any()) {
                ThisCacheProvider.UserClaimsSemaphore.Wait();
                List<Expression<Func<UserClaim, object>>> included = new List<Expression<Func<UserClaim, object>>>(includes)
                {
                    u => u.AuthorizationClaim
                };

                claims = (await this.DataRepo.GetUserClaimsRepository().GetAllAsync(token: cancellationToken, includes: included.ToArray())).Adapt<IEnumerable<UserClaimDto>>();
                if (claims is null)
                    claims = Enumerable.Empty<UserClaimDto>();
                this.Cache.Set(ThisCacheProvider.UserClaimsKey, claims);
                ThisCacheProvider.UserClaimsSemaphore.Release();
            }

            return claims;
        }

        /// <summary>
        /// Gets Users who are marked as active from the cache, or populates the cache from the DB
        /// </summary>
        /// <param name="refresh">Whether to force updating cache from DB</param>
        /// <param name="cancellationToken"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<UserDto>> GetUsers(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<User, object>>[] includes)
        {
            IEnumerable<UserDto>? items;

            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.UsersSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.UsersKey);
                ThisCacheProvider.UsersSemaphore.Release();
            }

            try
            {
                // we use thread lock because GetOrCreate is not thread safe
                // see https://github.com/dotnet/runtime/issues/36499
                await ThisCacheProvider.UsersSemaphore.WaitAsync();

                items = await this.Cache.GetOrCreateAsync(ThisCacheProvider.UsersKey, async c =>
                {
                    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.EndOfDay(),
                        SlidingExpiration = TimeSpan.FromHours(8),
                        Size = 100,
                    };

                    return (await this.DataRepo.GetUsersRepository().GetActiveAsync(token: cancellationToken, includes: includes)).Adapt<IEnumerable<UserDto>>();
                });
            }
            catch
            {
                throw;
            }
            finally
            {
                ThisCacheProvider.UsersSemaphore.Release();
            }

            return items ?? Enumerable.Empty<UserDto>();
        }

        public async ValueTask<IEnumerable<BarcodeDto>> GetBarcodes(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<Barcode, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.BarcodesSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.BarcodesKey);
                ThisCacheProvider.BarcodesSemaphore.Release();
            }
            return await this.GetOrSetCachedDbCollection<Barcode, BarcodeDto>(ThisCacheProvider.BarcodesKey, ThisCacheProvider.BarcodesSemaphore, this.DataRepo.GetBarcodesRepository()
                ,cancellationToken: cancellationToken, includes: includes);
        }

        public async ValueTask<IEnumerable<JobTypeDto>> GetJobTypes(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<JobType, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.JobTypesSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.JobTypesKey);
                ThisCacheProvider.JobTypesSemaphore.Release();
            }
            return await this.GetOrSetCachedDbCollection<JobType, JobTypeDto>(ThisCacheProvider.JobTypesKey, ThisCacheProvider.JobTypesSemaphore, this.DataRepo.GetJobTypesRepository()
                , cancellationToken: cancellationToken, includes: includes);
        }

        public async ValueTask<IEnumerable<JobStepDto>> GetJobSteps(bool refresh = false, CancellationToken cancellationToken = default, params Expression<Func<JobStep, object>>[] includes)
        {
            if (refresh)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ThisCacheProvider.JobStepsSemaphore.WaitAsync();
                this.Cache.Remove(ThisCacheProvider.JobStepsKey);
                ThisCacheProvider.JobStepsSemaphore.Release();
            }
            return await this.GetOrSetCachedDbCollection<JobStep, JobStepDto>(ThisCacheProvider.JobStepsKey, ThisCacheProvider.JobStepsSemaphore, this.DataRepo.GetJobStepsRepository()
                , cancellationToken: cancellationToken, includes: includes);
        }

        #region Generic Methods
        public async Task RefeshAll(bool force = false)
        {
#warning NOT IMPLEMENTED
            throw new NotImplementedException();
        }

        public void AddToCache<T>(string key, T value, SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            this.Cache.Set(key, value);
            semaphore.Release();
        }

        public void AddToCachedCollection<T>(string key, T value, SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            IEnumerable<T>? values;

            this.Cache.TryGetValue(key, out values);
            values ??= Enumerable.Empty<T>();
            List<T> newValues = new(values);

            newValues.Add(value);
            this.Cache.Set(key, newValues);
            semaphore.Release();
        }

        public async Task<IEnumerable<T>> GetCachedCollection<T>(string key, SemaphoreSlim semaphore)
        {
            IEnumerable<T>? items;
            bool isAvaiable = this.Cache.TryGetValue(key, out items);

            if (isAvaiable)
                return items ?? Enumerable.Empty<T>();

            try
            {
                await semaphore.WaitAsync();
                isAvaiable = this.Cache.TryGetValue(key, out items);

                if (isAvaiable) items = items ?? Enumerable.Empty<T>();
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }

            return items ?? Enumerable.Empty<T>();
        }

        public async Task<IEnumerable<T2>> GetOrSetCachedDbCollection<T1, T2>(string key, SemaphoreSlim semaphore, IDataRepository<T1> data, int size = 100, CancellationToken cancellationToken = default, params Expression<Func<T1, object>>[] includes) where T1 : class, IEntityModel
        {
            IEnumerable<T2>? items;

            try
            {
                // we use thread lock because GetOrCreate is not thread safe
                // see https://github.com/dotnet/runtime/issues/36499
                await semaphore.WaitAsync();

                items = await this.Cache.GetOrCreateAsync(key, async c =>
                {
                    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.EndOfDay(),
                        SlidingExpiration = TimeSpan.FromHours(8),
                        Size = size,
                    };
                    ISorting<T1> sort = new Sorting<T1>();
                    sort.Sorts.Add(new Sort<T1>() { SortBy = (i) => i.RowId });
                    return (await data.GetAllAsync(token: cancellationToken, includes: includes, sorting: sort)).Adapt<IEnumerable<T2>>();
                });
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }

            return items ?? Enumerable.Empty<T2>();
        }
        #endregion Generic Methods
    }
}
