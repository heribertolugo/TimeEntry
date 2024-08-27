using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

internal sealed class GetRefreshTokenDto: CanJson<GetRefreshTokenDto>
{
    public Guid DeviceId { get; set; }
    public string? RefreshToken { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
    public string? Password {  get; set; } = string.Empty;
}
