namespace TimeClock.Data.Models;

public partial class DepartmentsToLocation : IEntityModel
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public Guid LocationId { get; set; }

    public Guid DepartmentId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<EquipmentsToDepartmentLocation> EquipmentsToDepartmentLocations { get; set; } = new List<EquipmentsToDepartmentLocation>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
