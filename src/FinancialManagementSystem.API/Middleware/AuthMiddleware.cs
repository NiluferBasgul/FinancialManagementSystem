using FinancialManagementSystem.Core.Interfaces;
using System.Security.Claims;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var userId = authService.ValidateToken(token);
                if (userId != null)
                {
                    _logger.LogInformation($"User authenticated: {userId}");
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                        new Claim(ClaimTypes.Name, userId.Value.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, "Token");
                    context.User = new ClaimsPrincipal(identity);
                }
                else
                {
                    _logger.LogWarning("Invalid token provided");
                }
            }
        }

        await _next(context);
    }
}