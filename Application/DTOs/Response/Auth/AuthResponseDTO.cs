using Domain.Enums;

namespace Application.DTOs.Response.Auth
{
    public class AuthResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
