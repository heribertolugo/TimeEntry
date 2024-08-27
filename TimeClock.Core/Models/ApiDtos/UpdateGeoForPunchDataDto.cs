using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal sealed class UpdateGeoForPunchDataDto : CanJson<UpdateGeoForPunchDataDto>
{
    public Guid DeviceId { get; set; }
    public Guid PunchEntryId { get; set; }
    public Guid PunchEntryHistoryId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid UserId { get; set; }
    public Guid ActionById { get; set; }
}
