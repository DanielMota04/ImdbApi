using Application.DTOs.Pagination;
using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public UsersController(IUserService service, IHttpContextAccessor httpContextAcessor)
        {
            _service = service;
            _httpContextAcessor = httpContextAcessor;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers(
            [FromQuery] PaginationParams paginationParams,
            [FromQuery] Roles? role)
        {
            var users = await _service.GetAllUsers(paginationParams, role);
            return Ok(users);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUserRequestDTO dto)
        {
            var loggedUser = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.UpdateUser(id, dto, loggedUser);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            await _service.DeactivateUser(id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("/me")]
        public async Task<IActionResult> DeactivateMe()
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.DeactivateMe(userId);
            return NoContent();
        }

    }
}
