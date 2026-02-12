namespace Application.DTOs.Request.Auth
{
    public class AuthLoginRequestDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
