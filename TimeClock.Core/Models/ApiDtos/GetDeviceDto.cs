using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal class GetDeviceDto : CanJson<GetDeviceDto>
{
    public Guid DeviceId { get; set; }
    public bool ForceRefresh { get; set; }
    public bool IncludeLocation { get; set; }
    public bool IncludeDepartment { get; set; }
    public bool IncludeConfiguredBy { get; set; }
}
