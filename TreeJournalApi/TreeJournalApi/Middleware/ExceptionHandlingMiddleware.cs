using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly MyDbContext _context;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, MyDbContext context)
    {
        _next = next;
        _logger = logger;
        _context = context;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SecureException ex)
        {
            await HandleSecureExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGeneralExceptionAsync(context, ex);
        }
    }

    private async Task HandleSecureExceptionAsync(HttpContext context, SecureException ex)
    {
        var eventId = Guid.NewGuid().ToString();
        var logEntry = new ExceptionLog
        {
            EventId = eventId,
            Timestamp = DateTime.UtcNow,
            QueryParameters = context.Request.QueryString.ToString(),
            BodyParameters = await new StreamReader(context.Request.Body).ReadToEndAsync(),
            StackTrace = ex.StackTrace
        };

        _context.ExceptionLogs.Add(logEntry);
        await _context.SaveChangesAsync();

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"type\": \"{ex.GetType().Name}\", \"id\": \"{eventId}\", \"data\": {{\"message\": \"{ex.Message}\"}}}}");
    }

    private async Task HandleGeneralExceptionAsync(HttpContext context, Exception ex)
    {
        var eventId = Guid.NewGuid().ToString();
        var logEntry = new ExceptionLog
        {
            EventId = eventId,
            Timestamp = DateTime.UtcNow,
            QueryParameters = context.Request.QueryString.ToString(),
            BodyParameters = await new StreamReader(context.Request.Body).ReadToEndAsync(),
            StackTrace = ex.StackTrace
        };

        _context.ExceptionLogs.Add(logEntry);
        await _context.SaveChangesAsync();

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"type\": \"Exception\", \"id\": \"{eventId}\", \"data\": {{\"message\": \"Internal server error ID = {eventId}\"}}}}");
    }
}
