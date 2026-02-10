using Application.DTOs.Request.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
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
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginRequestDTO dto)
        {
            var token = await _service.LoginAsync(dto);
            return Ok(token);
        }
    }
}
