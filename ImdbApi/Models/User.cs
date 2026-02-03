using ImdbApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImdbApi.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Roles Role { get; set; }
        public bool IsActive { get; set; }
    }
}
