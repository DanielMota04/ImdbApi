using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Models;

namespace ImdbApi.Mappers
{
    public class AuthMapper
    {
        public User RegisterToEntity(AuthRegisterRequestDTO dto, string email, string password)
        {
            User user = new()
            {
                Name = dto.Name,
                Email = email,
                Password = password,
                Role = dto.Role
            };
            return user;
        }

        public AuthResponseDTO EntityToResponse(User entity)
        {
            AuthResponseDTO dto = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Role = entity.Role
            };

            return dto;
        }

    }
}
