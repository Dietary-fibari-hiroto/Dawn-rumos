using Microsoft.AspNetCore.Mvc;
using rumos_server.Externals.GrpcClients;
using rumos_server.Externals.MqttClients;
using rumos_server.Features.Interface;
using rumos_server.Features.Models;
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
        private readonly GrpcService _grpcService;
        public DeviceController(IDeviceService service, GrpcService grpcService)
        {
            _service = service;
            _grpcService = grpcService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetDeviceAsync());
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _service.GetDeviceAsync(id);
            return device == null ? NotFound() : Ok(device);
        }

        /*---tp-link制御---*/
        //tpplugの状態取得
        [HttpGet("tp/state/{id}")]
        public async Task<IActionResult> GetTpState(int id)
        {
            //受け取ったidを使ってIPだけ取得
            string? ip = await _service.GetTpIpAsync(id);
            if(ip == null) return NotFound("IPの取得に失敗");
            RGetStatusReply res = await _grpcService.GetDevicePower(ip);
            return Ok(res);
        }

        //tpplugをOn/Off切り替えるエンドポイント
        [HttpPost("tp/supply/{id}")]
        public async Task<IActionResult> ChangePowerSupply(int id)
        {
            //受け取ったidを使ってIPだけ取得
            string? ip = await _service.GetTpIpAsync(id);
            if (ip == null) { return NotFound("IP取得失敗"); }
            //取得したIPを使ってgRPCリクエスト
            RSetPowerReply res = await _grpcService.PowerSupply(ip);
            return Ok(res);
            ;
        }
        
    }

    //Luminaエンドポイント
    [Route("/api/[controller]")]
    public class LuminaController : ControllerBase
    {
        private readonly IDeviceService _service;
        private readonly MqttService _mqttService;
        private readonly IPresetService _presetService;
        public LuminaController(IDeviceService service,MqttService mqttService,IPresetService presetService)
        {
            _service = service;
            _mqttService = mqttService;
            _presetService = presetService;
        }

        [HttpPost("all")]
        public async Task<IActionResult> SetColorForAll(LedColor color,CancellationToken ct)
        {
            await _mqttService.SendColorAsyncForAll(color, ct);
            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SetColor([FromBody]LedColor color,int id,CancellationToken ct)
        {
            string? deviceName = await _service.GetDeviceNameAsync(id);
            if (deviceName == null) return NotFound();

            await _mqttService.SendColorAsync(color, deviceName);
            return Ok();
        }

        /*magicRoutin用エンドポイント*/
        //全情報取得
        [HttpGet("magicroutin")]
        public async Task<IActionResult> GetMagicRoutin(CancellationToken ct)
        {
            var presets = await _presetService.GetAllPresetAsync();
            return presets == null ? NotFound() : Ok(presets); 
        }
        //登録
        [HttpPost("magicroutin")]
        public async Task<IActionResult> PostMagicRoutin([FromBody]Preset preset,CancellationToken ct)
        {
            return Ok();
        }
        //Mqtt実行
        [HttpPost("magicroutin/execution")]
        public async Task<IActionResult> ExeMagicRoutin(int id,CancellationToken ct)
        {
            List<Preset_device_map> exeValue = await _presetService.GetMapsByIdAsync(id);
            foreach (Preset_device_map item in exeValue)
            {
                Console.WriteLine($"PresetId: {item.Preset_id}, DeviceId: {item.Device_id},DeviceName:{item.Device.Name}");
                //新しいcolorに格納
                LedColor color = new LedColor
                {
                    R = item.R,
                    G = item.G,
                    B = item.B,
                    Brightness = item.Brightness,
                };
                //colorを使ってMqttリクエスト
                await _mqttService.SendColorAsync(color, item.Device.Name);
            }
            
            return exeValue == null ? NotFound() : Ok(exeValue);
        }
        

      
    }

}
