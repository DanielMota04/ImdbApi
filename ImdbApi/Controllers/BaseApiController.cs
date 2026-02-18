using Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Value is null) return NoContent();
                return Ok(result.Value);
            }

            var firstError = result.Errors.FirstOrDefault();

            return firstError switch
            {
                NotFoundError => NotFound(new { message = firstError.Message }),
                ConflictError => Conflict(new { message = firstError.Message }),
                ForbiddenError => StatusCode(403, new { message = firstError.Message }),
                UnauthorizedError => Unauthorized(new { message = firstError.Message }),
                _ => BadRequest(new { errors = result.Errors.Select(e => e.Message) })
            };
        }
    }
}
