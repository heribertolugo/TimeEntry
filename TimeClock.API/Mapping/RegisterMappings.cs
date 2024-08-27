using Mapster;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.Api.Mapping
{
    public static class RegisterMappings
    {
        public static void SetGlobal()
        {
            TypeAdapterConfig.GlobalSettings.Default
                .PreserveReference(true);
        }

        public static void RegisterAllMappings()
        {
            SetGlobal();
            RegisterPunchEntryMapping();
            RegisterUserMapping();
            RegisterUserClaimsMapping();
            RegisterAuthClaimsMapping();
            RegisterDepartmentMapping();
            RegisterLocationMapping();
            RegisterDeviceMapping();
            RegisterDepartmentToLocationMapping();
            RegisterEquipmentMapping();
            RegisterEquipmentTypeMapping();
            RegisterEquipmentToLocationMapping();
            RegisterEquipmentToUserMapping();
            RegisterWorkPeriodMapping();
            RegisterWorkPeriodJobTypeMapping();
            RegisterJobTypeStepToUserMapping();
            RegisterTimeOnlyMapping();
        }

        public static void RegisterTimeOnlyMapping()
        {
            TypeAdapterConfig<DateOnly, DateOnly>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new DateOnly(src.Year, src.Month, src.Day) );
            TypeAdapterConfig<DateOnly, DateTime>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new DateTime(src.Year, src.Month, src.Day));
            TypeAdapterConfig<DateTime, DateOnly>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new DateOnly(src.Year, src.Month, src.Day));
        }

        #region Punch Entry
        public static void RegisterPunchEntryMapping()
        {
            TypeAdapterConfig<PunchEntry, PunchEntryDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new PunchEntryDto(
                        src.Id, // id
                        src.UserId, // userId
                        GetDeviceIdFromPunchEntry(src), // deviceId
                        GetPunchTypeFromPunchEntry(src), // punchType
                        GetDateTimeFromPunchEntry(src), // dateTime
                        GetEffectiveDateTimeFromPunchEntry(src), // effectiveDateTime
                        GetLatitudeFromPunchEntry(src), // latitude
                        GetLongitudeFromPunchEntry(src), // longitude
                        GetPunchActionFromPunchEntry(src), // punchAction
                        GetActionByIdFromPunchEntry(src), // actionById
                        GetUtcFromPunchEntry(src), // utcTimeStamp
                        src.User == null ? null : src.User.Adapt<UserDto>(), // user
                        GetDeviceFromPunchEntry(src), // device
                        GetActionByFromPunchEntry(src), // actionBy
                        new List<PunchEntriesHistoryDto>(src.PunchEntriesHistories.Adapt<IEnumerable<PunchEntriesHistoryDto>>()), // history
                        GetHistoryIdFromPunchEntry(src), // current state history id
                        src.CurrentState == null ? default : src.CurrentState.Id, // current state id,
                        src.WorkPeriodId, // work period id
                        src.WorkPeriod == null ? default : src.WorkPeriod.Adapt<WorkPeriodDto>(), // work period
                        GetNoteFromPunchEntry(src), // note
                        GetStableStateIdFromPunchEntry(src), // state state history id
                        GetStableStateFromPunchEntry(src) // state state history
                    )
                { PunchStatus = src.CurrentState.Status.Adapt<PunchStatusDto>(), StablePunchStatus = src.CurrentState.StableStatus.Adapt<PunchStatusDto>(),
                    StableDateTime = GetStableDateTimeFromPunchEntry(src), StableEffectiveDateTime = GetStableEffectiveDateTimeFromPunchEntry(src),
                    StableAction = GetStableActionFromPunchEntry(src),
                    JobTypeId = GetStableJobTypeIdFromPunchEntry(src), JobStepId = GetStableJobStepIdFromPunchEntry(src),
                    JobType = GetStableJobTypeFromPunchEntry(src), JobStep = GetStableJobStepFromPunchEntry(src)
                });
            TypeAdapterConfig<PunchEntriesHistory, PunchEntryDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new PunchEntryDto(
                        GetPunchIdFromHistory(src), // id
                        GetUserIdFromHistory(src), // userId
                        src.DeviceId, // deviceId
                        src.PunchType.Adapt<PunchTypeDto>(), // punchType
                        src.DateTime, // dateTime
                        src.EffectiveDateTime, // effectiveDateTime
                        src.Latitude, // latitude
                        src.Longitude, // longitude
                        src.Action.Adapt<PunchActionDto>(), // punchAction
                        src.ActionById, // actionById
                        src.UtcTimeStamp, // utcTimeStamp
                        GetUserFromHistory(src), // user
                        GetDeviceFromHistory(src), // device
                        GetActionByFromHistory(src), // actionBy
                        GetPunchHistoriesFromHistory(src), // history
                        GetCurrentStateHistoryIdFromHistory(src), // current state history id
                        GetCurrentStateIdFromHistory(src), // current state id
                        GetWorkPeriodIdFromHistory(src), // work period id
                        GetWorkPeriodFromHistory(src), // work period
                        src.Note, // note
                        GetStableStateIdFromHistory(src), // state state history id
                        GetStableStateFromHistory(src) // state state history
                    )
                { PunchStatus = src.PunchEntry.CurrentState.Status.Adapt<PunchStatusDto>(), StablePunchStatus = src.PunchEntry.CurrentState.StableStatus.Adapt<PunchStatusDto>(),
                    StableDateTime = GetStableDateTimeFromHistory(src), StableEffectiveDateTime = GetStableEffectiveDateTimeFromHistory(src),
                    StableAction = GetStableActionFromHistory(src), JobTypeId = src.JobTypeId, JobStepId = src.JobStepId,
                    JobType = GetStableJobTypeFromHistory(src), JobStep = GetStableJobStepFromHistory(src)
                });
            TypeAdapterConfig<PunchEntriesHistory, PunchEntriesHistoryDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new PunchEntriesHistoryDto() {
                        Id = src.Id, PunchEntryId = src.PunchEntryId, DeviceId = src.DeviceId,
                        PunchType = src.PunchType.Adapt<PunchTypeDto>(), DateTime = src.DateTime, 
                        EffectiveDateTime = src.EffectiveDateTime, Latitude = src.Latitude,
                        Longitude = src.Longitude, Action = src.Action.Adapt<PunchActionDto>(), 
                        ActionById = src.ActionById, UtcTimeStamp = src.UtcTimeStamp, 
                        ActionBy = GetUserFromHistory(src), Device = GetDeviceFromHistory(src), 
                        PunchEntry = null, Note = src.Note, JobStepId = src.JobStepId, JobTypeId = src.JobTypeId,
                        JobStep = GetJobStepFromHistory(src), JobType = GetJobTypeFromHistory(src)
                });

        }
        #region Get Using History
        private static JobTypeDto? GetStableJobTypeFromHistory(PunchEntriesHistory src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobType?.Adapt<JobTypeDto>();
        }
        private static JobStepDto? GetStableJobStepFromHistory(PunchEntriesHistory src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobStep?.Adapt<JobStepDto>();
        }
        private static JobTypeDto? GetJobTypeFromHistory(PunchEntriesHistory src)
        {
            return (src.JobType ?? src.CurrentState?.PunchEntriesHistory?.JobType)?.Adapt<JobTypeDto>();
        }
        private static JobStepDto? GetJobStepFromHistory(PunchEntriesHistory src)
        {
            return (src.JobStep ?? src.CurrentState?.PunchEntriesHistory?.JobStep)?.Adapt<JobStepDto>();
        }
        private static PunchActionDto? GetStableActionFromHistory(PunchEntriesHistory src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.Action.Adapt<PunchActionDto>();
        }
        private static Guid GetWorkPeriodIdFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.WorkPeriodId ?? history.CurrentState?.PunchEntry?.WorkPeriodId ?? Guid.Empty;
        }
        private static WorkPeriodDto? GetWorkPeriodFromHistory(PunchEntriesHistory history)
        {
            return (history.PunchEntry?.WorkPeriod ?? history.CurrentState?.PunchEntry?.WorkPeriod ?? null)?.Adapt<WorkPeriodDto>();
        }
        private static Guid GetPunchIdFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.Id ?? history.CurrentState?.PunchEntryId ?? Guid.Empty;
        }
        private static Guid GetUserIdFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.UserId ?? history.CurrentState?.PunchEntry?.UserId ?? Guid.Empty;
        }
        private static Guid GetCurrentStateIdFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.CurrentState?.Id ?? history.CurrentState?.Id ?? Guid.Empty;
        }
        private static Guid GetCurrentStateHistoryIdFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.CurrentState?.PunchEntriesHistoryId ?? history.CurrentState?.PunchEntriesHistoryId ?? Guid.Empty;
        }
        private static UserDto? GetUserFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.User?.Adapt<UserDto>();
        }
        private static DeviceDto? GetDeviceFromHistory(PunchEntriesHistory history)
        {
            return history.Device?.Adapt<DeviceDto>();
        }
        private static UserDto? GetActionByFromHistory(PunchEntriesHistory history)
        {
            return history.ActionBy?.Adapt<UserDto>();
        }
        private static IEnumerable<PunchEntriesHistoryDto> GetPunchHistoriesFromHistory(PunchEntriesHistory history)
        {
            return history.PunchEntry?.PunchEntriesHistories is null
                ? Enumerable.Empty<PunchEntriesHistoryDto>()
                : new List<PunchEntriesHistoryDto>(history.PunchEntry.PunchEntriesHistories.Adapt<IEnumerable<PunchEntriesHistoryDto>>());
        }
        private static Guid? GetStableStateIdFromHistory(PunchEntriesHistory history)
        {
            return history.CurrentState?.StablePunchEntriesHistoryId;
        }
        private static PunchEntriesHistoryDto? GetStableStateFromHistory(PunchEntriesHistory history)
        {
            return history.CurrentState?.StablePunchEntriesHistory?.Adapt<PunchEntriesHistoryDto>();
        }
        private static DateTime? GetStableDateTimeFromHistory(PunchEntriesHistory history)
        {
            return history.CurrentState?.StablePunchEntriesHistory?.DateTime;
        }
        private static DateTime? GetStableEffectiveDateTimeFromHistory(PunchEntriesHistory history)
        {
            return history.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime;
        }
        #endregion Get Using History
        #region Get Using Punch Entry
        private static JobTypeDto? GetStableJobTypeFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobType?.Adapt<JobTypeDto>();
        }
        private static JobStepDto? GetStableJobStepFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobStep?.Adapt<JobStepDto>();
        }
        private static Guid? GetStableJobTypeIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobType?.Id;
        }
        private static Guid? GetStableJobStepIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.JobStep?.Id;
        }
        private static PunchActionDto? GetStableActionFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.Action.Adapt<PunchActionDto>();
        }
        private static string? GetNoteFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.PunchEntriesHistory?.Note ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Note;
        }
        private static Guid GetDeviceIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.PunchEntriesHistory?.DeviceId ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.DeviceId ?? default;
        }
        private static PunchTypeDto GetPunchTypeFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.PunchType ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.PunchType)?.Adapt<PunchTypeDto>() ?? default;
        }
        private static DateTime GetDateTimeFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.DateTime ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.DateTime) ?? default;
        }
        private static DateTime GetEffectiveDateTimeFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.EffectiveDateTime ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.EffectiveDateTime) ?? default;
        }
        private static double GetLatitudeFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.Latitude ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Latitude) ?? default;
        }
        private static double GetLongitudeFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.Longitude ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Longitude) ?? default;
        }
        private static PunchActionDto GetPunchActionFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.Action ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Action)?.Adapt<PunchActionDto>() ?? default;
        }
        private static Guid GetActionByIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.PunchEntriesHistory.ActionById ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.ActionById ?? default;
        }
        private static DateTime GetUtcFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.UtcTimeStamp ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.UtcTimeStamp) ?? default;
        }
        private static DeviceDto? GetDeviceFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory?.Device ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Device)?.Adapt<DeviceDto>() ?? default;
        }
        private static UserDto? GetActionByFromPunchEntry(PunchEntry src)
        {
            return (src.CurrentState?.PunchEntriesHistory.ActionBy ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.ActionBy)?.Adapt<UserDto>() ?? default;
        }
        private static Guid GetHistoryIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.PunchEntriesHistory.Id ?? src.PunchEntriesHistories.MaxBy(h => h.DateTime)?.Id ?? default;
        }
        private static Guid? GetStableStateIdFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistoryId;
        }
        private static PunchEntriesHistoryDto? GetStableStateFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.Adapt<PunchEntriesHistoryDto?>();
        }
        private static DateTime? GetStableDateTimeFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.DateTime;
        }
        private static DateTime? GetStableEffectiveDateTimeFromPunchEntry(PunchEntry src)
        {
            return src.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime;
        }
        #endregion Get Using Punch Entry
        #endregion Punch Entry

        public static void RegisterUserMapping()
        {
            TypeAdapterConfig<User, UserDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapToConstructor(false)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterUserClaimsMapping()
        {
            TypeAdapterConfig<UserClaim, UserClaimDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(true)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterAuthClaimsMapping()
        {
            TypeAdapterConfig<AuthorizationClaim, AuthorizationClaimDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(true)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterDepartmentMapping()
        {
            TypeAdapterConfig<Department, DepartmentDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                ;
        }

        public static void RegisterLocationMapping()
        {
            TypeAdapterConfig<Location, LocationDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                ;
        }

        public static void RegisterDepartmentToLocationMapping()
        {
            TypeAdapterConfig<DepartmentsToLocation, DepartmentsToLocationDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(DepartmentsToLocation, DepartmentsToLocationsDto)
                ;
        }

        public static void RegisterDeviceMapping()
        {
            TypeAdapterConfig<Device, DeviceDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(DepartmentsToLocation, DepartmentsToLocationsDto)
                ;
        }

        public static void RegisterEquipmentMapping()
        {
            TypeAdapterConfig<Equipment, EquipmentDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterEquipmentTypeMapping()
        {
            TypeAdapterConfig<EquipmentType, EquipmentTypeDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterEquipmentToLocationMapping()
        {
            TypeAdapterConfig<EquipmentsToDepartmentLocation, EquipmentsToDepartmentLocationDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterEquipmentToUserMapping()
        {
            TypeAdapterConfig<EquipmentsToUser, EquipmentsToUserDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                //.MapToConstructor(false)
                //.Map(dest => dest.PunchEntries, src => src.PunchEntriesHistories)
                ;
        }

        public static void RegisterWorkPeriodMapping()
        {
            TypeAdapterConfig<WorkPeriod, WorkPeriodDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .Map(dest => dest.PunchEntries, src => src.PunchEntries);
        }

        public static void RegisterJobTypeStepToUserMapping()
        {
            TypeAdapterConfig<JobTypeStepToUser, JobTypeStepToUserDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible);
        }

        public static void RegisterJobTypeMapping()
        {
            TypeAdapterConfig<JobType, JobTypeDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible);
        }

        public static void RegisterJobStepMapping()
        {
            TypeAdapterConfig<JobStep, JobStepDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible);
        }

        #region WorkPeriodJobType
        public static void RegisterWorkPeriodJobTypeMapping()
        {
            TypeAdapterConfig<WorkPeriodJobTypeStep, WorkPeriodJobTypeStepDto>
                .ForType()
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .MapWith(src => new()
                {
                    Id = src.Id,
                    ActiveSince = src.ActiveSince,
                    WorkPeriodId = src.WorkPeriodId,
                    JobTypeId = src.JobTypeId,
                    JobTypeDescription = GetDescriptionFromJobType(src.JobType),
                    JobTypeIsActive = GetIsActiveFromJobType(src.JobType),
                    JobStepId = src.JobStepId,
                    JobStepDescription = GetDescriptionFromJobStep(src.JobStep),
                    JobStepIsActive = GetIsActiveFromJobStep(src.JobStep),
                    JobTypeJdeId = GetJdeIdFromJobStep(src.JobStep),
                    JobStepJdeId = GetJdeIdFromJobStep(src.JobStep)
                });
        }
        private static string? GetDescriptionFromJobType(JobType? jobType)
        {
            return jobType?.Description;
        }
        private static bool? GetIsActiveFromJobType(JobType? jobType)
        {
            return jobType?.IsActive;
        }
        private static string? GetDescriptionFromJobStep(JobStep? jobStep)
        {
            return jobStep?.Description;
        }
        private static bool? GetIsActiveFromJobStep(JobStep? jobStep)
        {
            return jobStep?.IsActive;
        }
        private static string? GetJdeIdFromJobType(JobType? src)
        {
            return src?.JdeId;
        }
        private static string? GetJdeIdFromJobStep(JobStep? src)
        {
            return src?.JdeId;
        }
        #endregion WorkPeriodJobType
    }
}
