using Domain.Enums;

namespace Application.DTOs.Request.Auth
{
    public class AuthRegisterRequestDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Roles Role { get; set; }
    }
}
