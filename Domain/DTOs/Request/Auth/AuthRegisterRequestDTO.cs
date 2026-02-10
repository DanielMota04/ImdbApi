using ImdbApi.Enums;

namespace ImdbApi.DTOs.Request.Auth
{
    public class AuthRegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }
}
