using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using ImdbApi.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ImdbApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMovieListRepository _movieListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserMapper _mapper;

        public UserService(IUserRepository userRepository, IMovieRepository movieRepository, IHttpContextAccessor httpContextAccessor, UserMapper mapper, IMovieListRepository movieListRepository)
        {
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _movieListRepository = movieListRepository;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers(Roles? role)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersReturn = users.Where(u => u.IsActive.Equals(true));

            if (role.HasValue)
            {
                return usersReturn.Where(u => u.Role.Equals(role)).Select(u => _mapper.ToUserResponse(u)).OrderBy(s => s.Name);
            }

            return usersReturn.Select(u => _mapper.ToUserResponse(u)).OrderBy(s => s.Name);
        }

        public async Task<UserResponse?> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return _mapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return true;
        }

        public async Task<double?> Vote(int movieId, double vote)
        {
            var movie = await _movieRepository.FindMovieById(movieId);
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var movieList = await _movieListRepository.ListMoviesByUserId(userId);

            if (!await _movieListRepository.IsMovieOnUserList(userId)) return null;

            movie.TotalRating += vote;
            movie.Votes += 1;
            movie.Rating = movie.TotalRating / movie.Votes;

            await _movieRepository.UpdateRating(movie);

            return movie.Rating;
        }
    }
}
