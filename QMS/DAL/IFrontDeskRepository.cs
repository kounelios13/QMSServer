using QMS.DTO;

namespace QMS.DAL
{
    public interface IFrontDeskRepository
    {
        IEnumerable<FrontDeskTerminal> GetAllDevices();
        void AddDevice(FrontDeskTerminal device);
        void RemoveDevice(string deviceId);
        FrontDeskTerminal? GetDeviceById(string deviceId);

        bool IsDeviceNameUnique(string deviceName);
    }
}
