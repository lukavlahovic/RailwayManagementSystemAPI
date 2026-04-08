using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.Exceptions;
using System.Net;
using System.Text.Json;

namespace RailwayManagementSystemAPI.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(ex, "Handled exception. TraceId: {TraceId}", context.TraceIdentifier);
            }

            var problem = new ProblemDetails
            {
                Title = statusCode == HttpStatusCode.InternalServerError ? "Internal Server Error" : "Request failed",
                Detail = statusCode == HttpStatusCode.InternalServerError ? "Something went wrong" : ex.Message,
                Status = (int)statusCode,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);
        }
    }
}
