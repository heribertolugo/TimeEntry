using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public enum PunchActionDto
{
    None = 0,
    Self,
    AdminPunch,
    AdminEdit,
    Void,
    NewRequest,
    EditRequest,
    AdminApproved,
    AdminRejected,
    SelfEquipmentSelect,
    AdminEquipmentSelect
}

/// <summary>
/// Extension methods and helpers for PunchActionDto
/// </summary>
internal static class PunchActionDtoEx
{
    public static PunchActionDto[] PositiveStates()
    {
        return [PunchActionDto.Self, PunchActionDto.AdminPunch, PunchActionDto.AdminEdit, PunchActionDto.AdminApproved, PunchActionDto.SelfEquipmentSelect, PunchActionDto.AdminEquipmentSelect];
    }
}
