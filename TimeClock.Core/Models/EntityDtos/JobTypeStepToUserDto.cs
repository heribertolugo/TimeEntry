namespace TimeClock.Core.Models.EntityDtos;
public partial class JobTypeStepToUserDto : CanJson<JobTypeStepToUserDto>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public Guid UserId { get; set; }
    public JobTypeDto? JobType { get; set; }
    public JobStepDto? JobStep { get; set; }
    public UserDto? User { get; set; }
}
