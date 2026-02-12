using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Domain.Models;

namespace Application.Mappers
{
    public class AuthMapper
    {
        public static User RegisterToEntity(AuthRegisterRequestDTO dto, string email, string password)
        {
            User user = new()
            {
                Name = dto.Name,
                Email = email,
                Password = password,
                Role = dto.Role,
                IsActive = true
            };
            return user;
        }

        public static AuthResponseDTO EntityToResponse(User entity)
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
