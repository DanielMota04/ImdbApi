using ImdbApi.DTOs.Response.Exceptions;
using ImdbApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace ImdbApi.MIddlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();
            
            switch (ex)
            {
                case ConflictException:
                    response.StatusCode = 409;
                    response.Message = ex.Message;
                    break;
                case DomainException:
                    response.StatusCode = 400;
                    response.Message = ex.Message;
                    break;
                case ForbiddenException:
                    response.StatusCode = 403;
                    response.Message = ex.Message;
                    break;
                case ResourceNotFoundException:
                    response.StatusCode = 404;
                    response.Message = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    response.StatusCode = 401;
                    response.Message = ex.Message;
                    break;
                case FluentValidation.ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = ex.Message;
                    break;
                default:
                    _logger.LogError(ex, "Internal error.");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Internal error. Try again latter";
                    break;

            }
            context.Response.StatusCode = response.StatusCode;
            var jsonResult = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResult);
        }
    }
}
