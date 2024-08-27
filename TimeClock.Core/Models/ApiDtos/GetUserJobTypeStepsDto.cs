using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal class GetUserJobTypeStepsDto : CanJson<GetUserJobTypeStepsDto>
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
}
