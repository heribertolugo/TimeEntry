using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.ApiDtos
{
    internal sealed class RegisterDeviceDto : CanJson<RegisterDeviceDto>
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
        public Guid DepartmentId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public bool IsPublic { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
