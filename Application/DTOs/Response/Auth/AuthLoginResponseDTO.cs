namespace Application.DTOs.Response.Auth
{
    public class AuthLoginResponseDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
