using System.IdentityModel.Tokens.Jwt;

namespace YarpApiGateway.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly string[] _publicPaths;

        public TokenRefreshMiddleware(
            RequestDelegate next,
            ILogger<TokenRefreshMiddleware> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

            _publicPaths = configuration.GetSection("PublicPaths").Get<string[]>() ?? [];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];
            var refreshToken = context.Request.Cookies["refreshToken"];

            // Checking if a refresh is needed
            if (!string.IsNullOrEmpty(refreshToken) &&
                (string.IsNullOrEmpty(accessToken) || !IsTokenValid(accessToken)))
            {
                try
                {
                    var newTokens = await RefreshTokensAsync(refreshToken);

                    if (newTokens != null)
                    {
                        // renew cookies
                        SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
                            .SetTokenCookies(context.Response, newTokens.Value.AccessToken, newTokens.Value.RefreshToken);

                        // Updating the Authorization header for downstream services
                        context.Request.Headers.Authorization = $"Bearer {newTokens.Value.AccessToken}";

                        _logger.LogDebug("Tokens refreshed successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to refresh tokens");

                    // for public routes not using tokens
                    if (!IsPublicPath(context.Request.Path))
                    {
                        ClearTokenCookies(context);
                    }
                }
            }

            await _next(context);
        }

        private bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                return jwtToken.ValidTo > DateTime.UtcNow.AddMinutes(-1);
            }
            catch
            {
                return false;
            }
        }

        private async Task<(string AccessToken, string RefreshToken)?> RefreshTokensAsync(string refreshToken)
        {
            var identityUrl = _configuration["IdentityService:Url"]!;
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{identityUrl}/api/auth/refresh-token");
            request.Headers.Add("Cookie", $"refreshToken={refreshToken}");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // retrieve new tokens from the response cookies
                var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
                var accessToken = ExtractCookieValue(setCookieHeaders, "accessToken");
                var newRefreshToken = ExtractCookieValue(setCookieHeaders, "refreshToken");

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(newRefreshToken))
                {
                    return (accessToken, newRefreshToken);
                }
            }

            return null;
        }

        private string? ExtractCookieValue(IEnumerable<string> setCookieHeaders, string cookieName)
        {
            foreach (var header in setCookieHeaders)
            {
                if (header.StartsWith($"{cookieName}="))
                {
                    var parts = header.Split(';')[0].Split('=');
                    if (parts.Length >= 2)
                    {
                        return string.Join("=", parts.Skip(1));
                    }
                }
            }
            return null;
        }

        private void ClearTokenCookies(HttpContext context)
        {
            context.Response.Cookies.Delete("accessToken");
            context.Response.Cookies.Delete("refreshToken");
        }

        private bool IsPublicPath(string path)
        {
            return _publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class TokenRefreshMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenRefreshMiddleware>();
        }
    }
}
