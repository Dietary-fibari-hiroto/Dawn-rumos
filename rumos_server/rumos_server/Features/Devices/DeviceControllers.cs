using Microsoft.AspNetCore.Mvc;
using rumos_server.Features.Interface;
namespace rumos_server.Features.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _service;
        public PlatformController(IPlatformService service){
            _service = service;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetPlatformAsync());
    }

    [ApiController]
    [Route("/api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _service;
        public DeviceController(IDeviceService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetDeviceAsync());
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _service.GetDeviceAsync(id);
            return device == null ? NotFound() : Ok(device);
        }
    }
}
