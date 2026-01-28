using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Models;

namespace ImdbApi.Mappers
{
    public class UserMapper
    {
        public UserResponse ToUserResponseDTO(User entity)
        {
            UserResponse dto = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Role = entity.Role
            };

            return dto;
        }

        public User RegisterToEntity(AuthRegisterRequestDTO dto, int id)
        {
            User entity = new()
            {
                Id = id,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

            return entity;
        }

        public AuthResponseDTO ToAuthResponseDTO(User entity)
        {
            AuthResponseDTO dto = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
            };

            return dto;
        }
    }
}
