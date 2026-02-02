using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces.Services;
using ImdbApi.Models;
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

        [Authorize]
        [HttpPut("{movieId}")]
        public async Task<ActionResult> Vote(int movieId, double vote)
        {
            var value = await _service.Vote(movieId, vote);

            if (value == null) 
                return BadRequest("Movie not in users list");
            
            return Ok(value);
        }

        // Adicionar query params para filtrar os usuários por Admin ou user comum
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers([FromQuery] Roles? role)
        {
            var users = await _service.GetAllUsers(role);
            return Ok(users);
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
