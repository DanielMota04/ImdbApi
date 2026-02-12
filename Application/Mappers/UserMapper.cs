using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.DTOs.Response.User;
using Domain.Models;

namespace Application.Mappers
{
    public class UserMapper
    {
        public static UserResponse ToUserResponseDTO(User entity)
        {
            UserResponse dto = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Role = entity.Role
            };

            return dto;
        }

        public static UserResponse ToUserResponse(User entity)
        {
            UserResponse dto = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Role = entity.Role
            };
            return dto;
        }

        public static User RegisterToEntity(AuthRegisterRequestDTO dto, int id)
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

        public static AuthResponseDTO ToAuthResponseDTO(User entity)
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
