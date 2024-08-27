using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IUnregisteredDevicesRepository : IDataRepository<UnregisteredDevice>
    {

    }

    public class UnregisteredDevicesRepository : DataRepository<UnregisteredDevice>, IUnregisteredDevicesRepository
    {
        internal UnregisteredDevicesRepository(TimeClockContext context) : base(context) { }
    }
}
