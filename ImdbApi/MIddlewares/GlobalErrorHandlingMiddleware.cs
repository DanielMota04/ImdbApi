using Application.DTOs.Response.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares
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
