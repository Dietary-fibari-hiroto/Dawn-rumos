using rumos_server.Features.Models;
using rumos_server.Features.Interface;
using rumos_server.Features.Repositories;

namespace rumos_server.Features.Services
{
    public class PlatformService:IPlatformService
    {
        private readonly IPlatformRepository _repo;

        public PlatformService(IPlatformRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Platform>> GetPlatformAsync() => _repo.GetAllAsync();
    }
}
