using ImdbApi.DTOs.Request.User;
using ImdbApi.DTOs.Response.User;
using ImdbApi.Enums;
using ImdbApi.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers([FromQuery] Roles? role)
        {
            var users = await _service.GetAllUsers(role);
            return Ok(users);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUserRequestDTO dto)
        {
            var response = await _service.UpdateUser(id, dto);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _service.DeactivateUser(id);
            if (!user) return NotFound();
            return NoContent();
        }

    }
}
