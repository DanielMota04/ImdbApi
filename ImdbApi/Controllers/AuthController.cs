using ImdbApi.DTOs.Request.Auth;
using ImdbApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthRegisterRequestDTO dto)
        {
            var user = await _service.RegisterAsync(dto);
            if (user == null) return BadRequest("Email already in use.");
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginRequestDTO dto)
        {
            var token = await _service.LoginAsync(dto);
            if (token == null) return Unauthorized("Invalid email or password");
            return Ok(token);
        }


    }
}
