using Application.Interfaces;
using Application.Mappers;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<AuthMapper>();
            services.AddScoped<UserMapper>();
            services.AddScoped<MovieMapper>();
            services.AddScoped<MovieListMapper>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMovieListService, MovieListService>();

            return services;
        }
    }
}
