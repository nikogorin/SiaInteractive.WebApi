using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SiaInteractive.Application.Dtos.Common;
using System.Net;
using System.Text.Json;

namespace SiaInteractive.WebApi.Modules
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, "Unhandled exception after response started. Path: {Path}", context.Request.Path);
                    throw;
                }

                await WriteErrorResponseAsync(context, ex);
            }
        }

        private async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
        {
            var (statusCode, detail) = MapException(context, ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new Response<object>
            {
                IsSuccess = false,
                Message = detail
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        private (HttpStatusCode StatusCode, string Detail) MapException(HttpContext context, Exception ex)
        {
            switch (ex)
            {
                case ValidationException validationException:
                    _logger.LogWarning(validationException, "Validation error. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.BadRequest, validationException.Message);
                case KeyNotFoundException keyNotFound:
                    _logger.LogWarning(keyNotFound, "Resource not found. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.NotFound, keyNotFound.Message);
                case DbUpdateConcurrencyException dbUpdateConcurrency:
                    _logger.LogWarning(dbUpdateConcurrency, "Concurrency conflict. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.Conflict, "The resource was modified or deleted by another process.");
                case DbUpdateException dbUpdate:
                    _logger.LogWarning(dbUpdate, "Duplicate key violation. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.Conflict, "A database update error occurred.");
                case InvalidOperationException invalidOperationException:
                    _logger.LogWarning(invalidOperationException, "Operation conflict. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.Conflict, invalidOperationException.Message);
                default:
                    _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);
                    return (HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            };
        }
    }
}
