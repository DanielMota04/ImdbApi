using Application.DTOs.Request.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
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
            return HandleResult(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginRequestDTO dto)
        {
            var token = await _service.LoginAsync(dto);
            return HandleResult(token);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _service.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
    }
}
