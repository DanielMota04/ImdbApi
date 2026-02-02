using ImdbApi.Models;

namespace ImdbApi.DTOs.Request
{
    public class UpdateUserRequestDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
