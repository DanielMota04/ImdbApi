using ImdbApi.Data;
using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Models;

namespace ImdbApi.Repository
{
    public interface IUserRepository
    {
        public Task<List<UserResponse>> GetUsers(); //encontrar todos os users -> apenas adm
        public Task<UserResponse> GetUserById(int id); //busca o user pelo id
        public Task<bool> ExistsByEmail(string email);
        public Task<AuthLoginResponseDTO> Login(AuthLoginRequestDTO request);
        public Task<AuthResponseDTO> Register(AuthRegisterRequestDTO request);
    }
}
