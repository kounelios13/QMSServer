using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QMS.Db;
using QMS.DTO;
namespace QMS.DAL;

public class FrontDeskRepository : IFrontDeskRepository
{
    private readonly QmsDbContext _context;

    public FrontDeskRepository(QmsDbContext context)
    {
        _context = context;
    }

    public IEnumerable<FrontDeskTerminal> GetAllDevices()
    {
        return _context.FrontDeskTerminals.AsNoTracking().ToList();
    }

    public void AddDevice(FrontDeskTerminal device)
    {
        _context.FrontDeskTerminals.Add(device);
        _context.SaveChanges();
    }

    public void RemoveDevice(string deviceId)
    {
        var device = _context.FrontDeskTerminals.FirstOrDefault(d => d.DeviceId == deviceId);
        if (device != null)
        {
            _context.FrontDeskTerminals.Remove(device);
            _context.SaveChanges();
        }
    }

    public FrontDeskTerminal? GetDeviceById(string deviceId)
    {
        return _context.FrontDeskTerminals.AsNoTracking().FirstOrDefault(d => d.DeviceId == deviceId);
    }

    public bool IsDeviceNameUnique(string deviceName)
    {
        return !_context.FrontDeskTerminals.Any(d => d.DeviceName == deviceName);
    }
}
