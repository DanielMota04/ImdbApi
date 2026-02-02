using ImdbApi.Enums;

namespace ImdbApi.DTOs.Response
{
    public class AuthResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
