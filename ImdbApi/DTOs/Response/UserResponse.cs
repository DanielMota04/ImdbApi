using ImdbApi.Models;

namespace ImdbApi.DTOs.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Role { get; set; }
    }
}
