using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal class GenericGetDto : CanJson<GenericGetDto>
{
    public Guid DeviceId { get; set; }
    public Guid RequestedById { get; set; }
    public DateTime Timestamp { get; set; }
}
