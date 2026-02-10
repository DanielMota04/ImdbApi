using ImdbApi.Enums;

namespace ImdbApi.DTOs.Response.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Role { get; set; }
    }
}
