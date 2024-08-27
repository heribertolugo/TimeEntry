namespace TimeClock.Data.Models;

public enum PunchAction
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
/// Extension methods and helpers for PunchAction
/// </summary>
public static class PunchActionEx
{
    public static PunchAction[] StableStates => [PunchAction.Self, PunchAction.AdminPunch, PunchAction.AdminEdit, PunchAction.AdminApproved, PunchAction.SelfEquipmentSelect, PunchAction.AdminEquipmentSelect];
}