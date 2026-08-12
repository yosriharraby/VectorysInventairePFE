using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace VectorysInventaire.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception non gérée: {Message}", ex.Message);
            await WriteErrorAsync(context, ex);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object body;
        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                body = new { error = "Unauthorized", message = exception.Message, statusCode = 401 };
                break;
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                body = new
                {
                    error = "Validation",
                    message = "Données invalides.",
                    statusCode = 400,
                    errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                };
                break;
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                body = new { error = "NotFound", message = exception.Message, statusCode = 404 };
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                body = new { error = "BadRequest", message = exception.Message, statusCode = 400 };
                break;
            case DbUpdateException dbUpdate when IsDuplicateKey(dbUpdate):
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                body = new
                {
                    error = "Conflict",
                    message = "Conflit de données : une entrée similaire existe déjà en base.",
                    statusCode = 409
                };
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                body = new
                {
                    error = "ServerError",
                    message = "Une erreur interne s'est produite.",
                    statusCode = 500
                };
                break;
        }

        var json = JsonSerializer.Serialize(body);
        await context.Response.WriteAsync(json);
    }

    private static bool IsDuplicateKey(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains("2601", StringComparison.Ordinal)
               || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
               || message.Contains("clé en double", StringComparison.OrdinalIgnoreCase);
    }
}
