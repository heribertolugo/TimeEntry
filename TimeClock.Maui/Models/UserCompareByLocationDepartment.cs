using TimeClock.Core.Models.EntityDtos;

namespace TimeClock.Maui.Models
{
    internal class UserCompareByLocationDepartment : IComparer<UserDto>
    {
        public UserCompareByLocationDepartment()
        {

        }
        public UserCompareByLocationDepartment(Guid locationId, Guid departmentID)
        {
            this.LocationId = locationId;
            this.DepartmentId = departmentID;
        }
        public int Compare(UserDto? x, UserDto? y)
        {
            DepartmentsToLocationDto? locationX = x?.GetLastPunchEntry()?.Device?.DepartmentsToLocations;
            DepartmentsToLocationDto? locationY = y?.GetLastPunchEntry()?.Device?.DepartmentsToLocations;
            int xValue = 0;
            int yValue = 0;

            if (locationX == null || locationY == null)
                return locationX is null && locationY is null ? 0 : (locationX is null ? -1 : 1);
            if (locationX.LocationId == this.LocationId && locationX.DepartmentId == this.DepartmentId)
                xValue = 1;
            if (locationY.LocationId == this.LocationId && locationY.DepartmentId == this.DepartmentId)
                yValue = 1;
            return xValue == yValue ? 0 : (xValue > yValue ? 1 : -1);
        }

        public Guid LocationId { get; set; }
        public Guid DepartmentId {  get; set; }
    }
}
