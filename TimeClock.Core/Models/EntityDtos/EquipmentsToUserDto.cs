using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class EquipmentsToUserDto
{
    public Guid Id { get; set; }
    public DateTime LinkedOn { get; set; }
    public DateTime? UnLinkedOn { get; set; }
    public DateTime? LinkedOnEffective { get; set; }
    public DateTime? UnLinkedOnEffective { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid UserId { get; set; }
    public Guid LinkedById { get; set; }
    public Guid? UnlinkedById { get; set; }
    public EquipmentDto? Equipment { get; set; }
    public UserDto? User { get; set; }
    public UserDto? LinkedBy { get; set; }
    public UserDto? UnlinkedBy { get; set; }
    public TimeSpan LinkedDuration => this.UnLinkedOn?.Subtract(this.LinkedOn) ?? TimeSpan.Zero;
    public string LinkedDurationText
    {
        get
        {
            if (!this.UnLinkedOn.HasValue)
                return " - present";

            return $"  ({this.LinkedDuration.TotalHours:N2} hours)";
        }
    }
    public TimeSpan LinkedDurationEffective => this.UnLinkedOnEffective?.Subtract(this.LinkedOnEffective ?? this.LinkedOn) ?? TimeSpan.Zero;
    public string LinkedDurationTextEffective
    {
        get
        {
            if (!this.UnLinkedOnEffective.HasValue)
                return " - present";

            return $"  ({this.LinkedDurationEffective.TotalHours:N2} hours)";
        }
    }
}
