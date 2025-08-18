using QMS.DTO;

namespace QMS.DAL
{
    public interface IFrontDeskRepository
    {
        IEnumerable<FrontDeskTerminal> GetAllDevices();
        void AddDevice(FrontDeskTerminal device);
        void RemoveDevice(string deviceId);
        FrontDeskTerminal? GetDeviceById(string deviceId);
        
        void UpdateDeviceLastSeen(string deviceId, DateTime lastSeen);
        bool IsDeviceNameUnique(string deviceName);
    }
}
