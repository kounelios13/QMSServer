using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMS.DAL;
using QMS.DTO;

namespace QMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontDeskDeviceController : ControllerBase
    {
        private readonly ILogger<FrontDeskDeviceController> _logger;
        private readonly IFrontDeskRepository _frontDeskRepository;
        public FrontDeskDeviceController(ILogger<FrontDeskDeviceController> logger , IFrontDeskRepository repository)
        {
            _logger = logger;
            _frontDeskRepository = repository;
        }

        [HttpPost("Register")]
        public IActionResult RegisterDevice([FromBody] FrontDeskDeviceRegisterRequest payload)
        {
            if (string.IsNullOrEmpty(payload.DeviceId) && string.IsNullOrEmpty(payload.DeviceName))
            {
                return BadRequest("Device ID cannot be null or empty.");
            }

            if (!_frontDeskRepository.IsDeviceNameUnique(payload.DeviceName ?? ""))
            {
                return BadRequest("Terminal with given name already exists");
            }
            
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            var id = Guid.CreateVersion7();
            // Here you would typically save the device ID to a database or perform some action.
            _logger.LogInformation($"Device registered: {payload.DeviceId} with ip addr {remoteIpAddress}");
            var device = new FrontDeskTerminal
            {
                DeviceId = id.ToString(),
                DeviceName = payload.DeviceName ?? "Unnamed Device",
                //IpAddress = remoteIpAddress?.ToString() ?? "Unknown"
            };
            device.IPAddress = remoteIpAddress?.ToString() ?? "Unknown";
            _frontDeskRepository.AddDevice(device);
            return Ok(device);
        }

        [HttpGet("Devices")]
        public IActionResult GetAllDevices()
        {
            var devices = _frontDeskRepository.GetAllDevices();
            return Ok(devices);
        }


    }
}
