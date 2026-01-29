using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using rumos_server.Externals.GrpcClients;
using rumos_server.Features.DTOs;
using rumos_server.Features.Interface;
using rumos_server.Features.Logs;
using rumos_server.Features.Models;
using Sprache;
using rumos_server.Externals.IoTHubClients;
using System.Drawing;
namespace rumos_server.Features.Devices
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

        [HttpPost("register")]
        public async Task<IActionResult> CreateDevice(CreateDeviceDto request)
        {
            Device created = await _service.CreateDeviceAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteDeviceAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

    }

    //MagicRoutinエンドポイント
    [Route("/api/[controller]")]
    public class MagicRoutin : ControllerBase {
        private readonly IPresetService _service;
        public MagicRoutin(IPresetService service)
        {
            _service = service;
        }

        [HttpGet]//一覧取得
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllPresetAsync());
        [HttpPost]//登録処理
        public async Task<IActionResult> Create([FromForm]PresetCreateDto request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest("画像がありません");

            Preset created = await _service.CreateAsync(request);
            return Created($"/MagicRoutin/{created.Id}", created);
        }

        [HttpDelete("{id}")]//削除処理
        public async Task<IActionResult> DeletePreset(int id)
        {
            var result = await _service.DeletePresetAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpPost("routine/{id}")]
        public async Task<IActionResult> CreateRoutine([FromBody]List<DeviceDto> list,int id)
        {
            if (list == null) return BadRequest("データが入っていません。");
            bool isSuccess = await _service.RegisterDeviceMapAsync(list,id);
            return Ok(isSuccess);
        }
        
    }


    //Luminaエンドポイント
    [Route("/api/[controller]")]
    public class LuminaController : ControllerBase
    {
        private readonly IDeviceService _service;
        private readonly IoTHubService _iotHubService;
        private readonly IPresetService _presetService;
        private readonly ILogService _logService;

        public LuminaController(
            IDeviceService service,
            IoTHubService iotHubService,
            IPresetService presetService,
            ILogService logService)
        {
            _service = service;
            _iotHubService = iotHubService;
            _presetService = presetService;
            _logService = logService;
        }

        //プラットフォームベースですべてのデバイス取得
        [HttpGet("deviceswithplatform")]
        public async Task<IActionResult> GetDevicesGroupedByPlatform()
        {
            return Ok(await _service.GetAllDevicesWithPlatformAsync());
        }

        [HttpPost("all")]
        public async Task<IActionResult> SetColorForAll([FromBody] LedColor color, CancellationToken ct)
        {
            // IoT Hub では全デバイスのID一覧が必要
            var allDevices = await _service.GetDeviceAsync();
            var deviceNames = allDevices.Select(d => d.Name).ToList();

            await _iotHubService.SendColorToAllAsync(color, deviceNames, ct);

            await _logService.LogInfoAsync(
                $"全デバイスの色を変更: R={color.R}, G={color.G}, B={color.B}, Brightness={color.Brightness}",
                category: "lumina"
            );

            return Ok();
        }

        //一つの端末に対して制御をおこなうエンドポイント
        [HttpPost("{id}")]
        public async Task<IActionResult> SetColor([FromBody] LedColor color, int id, CancellationToken ct)
        {
            string? deviceName = await _service.GetDeviceNameAsync(id);
            if (deviceName == null)
            {
                await _logService.LogWarningAsync(
                    $"デバイスが見つかりません: ID={id}",
                    category: "lumina"
                );
                return NotFound();
            }

            // ← IoT Hub の deviceId = deviceName として送信
            await _iotHubService.SendColorAsync(color, deviceName, ct);

            await _logService.LogInfoAsync(
                $"デバイスの色を変更: {deviceName}",
                category: "lumina",
                deviceId: id.ToString(),
                metadata: new Dictionary<string, object>
                {
                    ["r"] = color.R,
                    ["g"] = color.G,
                    ["b"] = color.B,
                    ["brightness"] = color.Brightness
                }
            );

            return Ok();
        }

        /*magicRoutin用エンドポイント*/
        [HttpPost("magicroutin/execution/{id}")]
        public async Task<IActionResult> ExeMagicRoutin(int id, CancellationToken ct)
        {
            List<Preset_device_map> exeValue = await _presetService.GetMapsByIdAsync(id);
            var allDevices = await _service.GetDeviceAsync();

            var presetIds = exeValue.Select(x => x.Device_id).ToHashSet();
            var devicesToOff = allDevices.Where(d => !presetIds.Contains(d.Id));

            // 消灯対象
            foreach (var dev in devicesToOff)
            {
                await _iotHubService.SendColorAsync(new LedColor(), dev.Name, ct);
            }

            // 点灯対象
            foreach (Preset_device_map item in exeValue)
            {
                Console.WriteLine($"PresetId: {item.Preset_id}, DeviceId: {item.Device_id}, DeviceName:{item.Device.Name}");

                LedColor color = new LedColor
                {
                    R = item.R,
                    G = item.G,
                    B = item.B,
                    Brightness = item.Brightness,
                };

                await _iotHubService.SendColorAsync(color, item.Device.Name, ct);
            }

            await _logService.LogInfoAsync(
                $"MagicRoutine実行: PresetID={id}, 対象デバイス数={exeValue.Count}, 消灯デバイス数={devicesToOff.Count()}",
                category: "lumina",
                metadata: new Dictionary<string, object>
                {
                    ["presetId"] = id,
                    ["targetDevices"] = exeValue.Select(x => x.Device.Name).ToList(),
                    ["offDevices"] = devicesToOff.Select(x => x.Name).ToList()
                }
            );

            return exeValue == null ? NotFound() : Ok(exeValue);
        }
    }

    [Route("/api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _roomService.GetAllRoomAsync());
        }

    }
    
}
