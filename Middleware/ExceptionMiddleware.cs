using Microsoft.AspNetCore.Http;
using System.Text.Json;
using URLShortner.API.DTOs;
using URLShortner.API.Exceptions;

namespace URLShortner.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }
        private static int GetStatusCode(Exception ex)
        {
            if (ex is DuplicateShortCodeException)
            {
                return StatusCodes.Status409Conflict;
            }
            else if (ex is ReservedShortCodeException)
            {
                return StatusCodes.Status400BadRequest;
            }
            else if (ex is ArgumentException)
            {
                return StatusCodes.Status400BadRequest;
            }
            else if (ex is KeyNotFoundException)
            {
                return StatusCodes.Status404NotFound;
            }
            else if (ex is UnauthorizedAccessException)
            {
                return StatusCodes.Status401Unauthorized;
            }
            else
            {
               return StatusCodes.Status500InternalServerError;
            }
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = GetStatusCode(ex);
                context.Response.ContentType = "application/json";
                var errorResponse = new ErrorResponseDto
                {
                    Status = context.Response.StatusCode,
                    Message = _environment.IsDevelopment() ? ex.Message : "An unexpected error occured",
                    TimeStamp = DateTime.UtcNow
                };
                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonResponse);

            }
        }
    }
}
