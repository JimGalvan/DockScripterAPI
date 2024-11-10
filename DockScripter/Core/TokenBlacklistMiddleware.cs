using DockScripter.Services.Interfaces;

namespace DockScripter.Core;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public TokenBlacklistMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklistService)
    {
        _next = next;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var path = context.Request.Path;
        var pathsToIgnore = new List<string> { "/api/Auth/Login", "/api/Auth/Register" };
        var shouldIgnore = pathsToIgnore.Contains(path);

        if (_tokenBlacklistService.IsTokenBlacklisted(token) && !shouldIgnore)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token is blacklisted.");
            return;
        }

        await _next(context);
    }
}