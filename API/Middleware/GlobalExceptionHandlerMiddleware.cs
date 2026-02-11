using BookingSystem.Api.Middleware;
using BookingSystem.Application.Exceptions;
using BookingSystem.Application;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookingSystem.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ErrorResponse
        {
            StatusCode = GetStatusCode(exception),
            Message = GetMessage(exception),
            Details = exception.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BookingConflictException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetMessage(Exception exception)
    {
        return exception switch
        {
            BookingConflictException => "Booking conflict: Room is already booked during this time.",
            KeyNotFoundException => "Resource not found.",
            ArgumentException => "Invalid input provided.",
            UnauthorizedAccessException => "Access denied.",
            _ => "An internal server error occurred."
        };
    }
}