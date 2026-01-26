using Microsoft.EntityFrameworkCore;
using rumos_server.Data;
using rumos_server.Features.Models;

namespace rumos_server.Features.Logs
{
    public class LogRepository : ILogRepository
    {
        private readonly CosmosDbContext _context;

        public LogRepository(CosmosDbContext context)
        {
            _context = context;
        }

        public async Task<LogEntry> CreateAsync(LogEntry log)
        {
            log.Id = Guid.NewGuid().ToString();
            log.Timestamp = DateTime.UtcNow;

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return log;
        }

        public async Task<IEnumerable<LogEntry>> GetAllAsync(int limit = 100)
        {
            return await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByCategoryAsync(string category, int limit = 100)
        {
            // パーティションキーを使うのでWithPartitionKeyを指定
            return await _context.Logs
                .WithPartitionKey(category)
                .Where(l => l.Category == category)
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByDeviceIdAsync(string deviceId, int limit = 100)
        {
            return await _context.Logs
                .Where(l => l.DeviceId == deviceId)
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime from, DateTime to, int limit = 100)
        {
            return await _context.Logs
                .Where(l => l.Timestamp >= from && l.Timestamp <= to)
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
    }
}
