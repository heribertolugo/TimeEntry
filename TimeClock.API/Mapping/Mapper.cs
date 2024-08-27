using Mapster;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data.Models;

namespace TimeClock.Api.Mapping
{
    internal static class Mapper
    {
        static Mapper()
        {
        }

        public static PunchEntryDto? Map(PunchEntry? punchEntry, PunchEntriesHistory? punchEntryHistory)
        {
            if (punchEntryHistory is null)
                return null;
            PunchEntryDto dto = punchEntryHistory.Adapt<PunchEntryDto>();
            
            if (punchEntry is not null)
                dto.UserId = punchEntry.UserId;
            return dto;
        }

        public static PunchEntryDto? Map(PunchEntriesCurrentState? punchEntriesCurrentState)
        {
            if (punchEntriesCurrentState is null)
                return null;
            return punchEntriesCurrentState.Adapt<PunchEntryDto>();
        }

        public static PunchEntriesHistoryDto? Map(PunchEntriesHistory? punchEntryHistory)
        {
            if (punchEntryHistory is null)
                return null;
            return punchEntryHistory.Adapt<PunchEntriesHistoryDto>();
        }

        public static PunchActionDto Map(Data.Models.PunchAction punchAction)
        {
            return punchAction.Adapt<PunchActionDto>();
        }

        public static PunchTypeDto Map(Data.Models.PunchType punchType)
        {
            return punchType.Adapt<PunchTypeDto>();
        }

        public static UserDto? Map(User? user)
        {
            return user?.Adapt<UserDto?>();
        }

        public static UserClaimDto? Map(UserClaim? claim)
        {
            if (claim is null)
                return null;
            return claim.Adapt<UserClaimDto?>();
        }

        public static AuthorizationClaimDto? Map(AuthorizationClaim claim)
        {
            if (claim is null)
                return null;
            return claim.Adapt<AuthorizationClaimDto>();
        }

        public static DepartmentDto? Map(Department? department)
        {
            if (department is null)
                return null;
            return department.Adapt<DepartmentDto>();
        }

        public static DeviceDto? Map(Data.Models.Device? device)
        {
            if (device is null)
                return null;
            return device.Adapt<DeviceDto>();
        }

        public static EquipmentDto? Map(Data.Models.Equipment? device)
        {
            if (device is null)
                return null;
            return device.Adapt<EquipmentDto>();
        }

        public static EquipmentsToDepartmentLocationDto? Map(Data.Models.EquipmentsToDepartmentLocation? equipmentsToLocation)
        {
            if (equipmentsToLocation is null)
                return null;
            return equipmentsToLocation.Adapt<EquipmentsToDepartmentLocationDto>();
        }

        public static EquipmentsToUserDto? Map(Data.Models.EquipmentsToUser? equipmentsToUser)
        {
            if (equipmentsToUser is null)
                return null;
            return equipmentsToUser.Adapt<EquipmentsToUserDto>();
        }

        public static EquipmentTypeDto? Map(Data.Models.EquipmentType? equipmentType)
        {
            if (equipmentType is null)
                return null;
            return equipmentType.Adapt<EquipmentTypeDto>();
        }

        public static LocationDto? Map(Data.Models.Location? location)
        {
            if (location is null)
                return null;
            return location.Adapt<LocationDto>();
        }

        public static DepartmentsToLocationDto? Map(Data.Models.DepartmentsToLocation? location)
        {
            if (location is null)
                return null;
            return location.Adapt<DepartmentsToLocationDto>();
        }

        public static WorkPeriodJobTypeStepDto? Map(Data.Models.WorkPeriodJobTypeStep? workPeriodJobType)
        {
            if (workPeriodJobType is null)
                return null;
            return workPeriodJobType.Adapt<WorkPeriodJobTypeStepDto>();
        }
    }
}
