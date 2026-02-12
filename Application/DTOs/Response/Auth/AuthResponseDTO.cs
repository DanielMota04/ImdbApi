using Domain.Enums;

namespace Application.DTOs.Response.Auth
{
    public class AuthResponseDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public Roles Role { get; set; }
    }
}
