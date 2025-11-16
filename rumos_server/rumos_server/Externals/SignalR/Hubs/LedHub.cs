using Microsoft.AspNetCore.SignalR;

namespace rumos_server.SignalR.Hubs
{
    public class LedHub:Hub
    {
        private readonly ILogger<LedHub> _logger;
        public LedHub(ILogger<LedHub> logger)
        {
            _logger  = logger;
        }
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //コネクト時の関数
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"🟢 クライアント接続: {connectionId}");
            await Clients.Caller.SendAsync("ReceivceMessage", "Server", "接続ありがとう！");
            await base.OnConnectedAsync();
        }

    }

    public class TestHub : Hub
    {
        private readonly ILogger<TestHub> _logger;

        public TestHub(ILogger<TestHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"🟢 クライアント接続: {connectionId}");
            await Clients.Caller.SendAsync("ReceiveMessage", "Server", "接続ありがとう！");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"🔴 クライアント切断: {connectionId}");
            if (exception != null)
                _logger.LogError(exception, "切断時にエラー発生");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            _logger.LogInformation($"📨 {user}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // ✅ テストデータ送信用メソッド（例）
        public async Task SendTestData()
        {
            var testData = new { time = DateTime.Now, status = "OK", value = 123 };
            _logger.LogInformation("🧪 テストデータ送信: {@testData}", testData);
            await Clients.All.SendAsync("ReceiveMessage", "Server", $"テストデータ: {testData}");
        }
    }
}
