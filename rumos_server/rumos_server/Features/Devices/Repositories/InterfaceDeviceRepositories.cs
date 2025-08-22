using rumos_server.Features.Models;

namespace rumos_server.Features.Interface;


    public interface IPlatformRepository
    {
        Task<IEnumerable<Platform>> GetAllAsync();
    }

