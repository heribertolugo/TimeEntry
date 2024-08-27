
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models;

public enum AuthorizationClaimType
{
    Unknown,
    CanSelectEquipment,
    CanViewOthersPunches,
    CanEditOthersPunches,
    CanConfigureApp,
    CanCreateEmployee
}
