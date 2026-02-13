namespace Application.DTOs.Response.Auth
{
    public class TokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
