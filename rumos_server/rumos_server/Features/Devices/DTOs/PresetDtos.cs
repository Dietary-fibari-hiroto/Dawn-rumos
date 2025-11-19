namespace rumos_server.Features.DTOs
{
    public class PresetDtos
    {
    }

    //作るときに使う
    public class PresetCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
    }

}
