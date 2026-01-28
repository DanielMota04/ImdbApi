using ImdbApi.Models;

namespace ImdbApi.DTOs.Request
{
    public class AuthRegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }
}
