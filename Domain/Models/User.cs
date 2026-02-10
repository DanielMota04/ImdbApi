using Domain.Enums;

namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Roles Role { get; set; }
        public bool IsActive { get; set; }
    }
}
