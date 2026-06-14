using IdentityService.Models.DTOs;
using IdentityService.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityService.Middleware
{
    public class TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];

            // if there is no access token, skip and go to next (could be public endpoint)
            if (string.IsNullOrEmpty(accessToken))
            {
                await next(context);
                return;
            }

            // check if token is valid
            if (!IsTokenValid(accessToken))
            {
                // try to refresh using refresh token
                var refreshToken = context.Request.Cookies["refreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        var newTokens = await RefreshTokensAsync(context, refreshToken);

                        if (newTokens != null)
                        {
                            // refresh cookies

                            SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
                                .SetTokenCookies(context.Response, newTokens.AccessToken!, newTokens.RefreshToken!);

                            logger.LogDebug("Tokens refreshed successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to refresh tokens");
                        // delete cookies
                        if (!IsPublicPath(context.Request.Path))
                        {
                            SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
                            .ClearTokenCookies(context.Response);
                        }
                    }
                }
            }

            await next(context);
        }

        private bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                return jwtToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        private async Task<RefreshTokenResponse?> RefreshTokensAsync(HttpContext context, string refreshToken)
        {
            // get service from DI container
            var tokenService = context.RequestServices.GetRequiredService<ITokenService>();

            try
            {
                var tokenResponse = await tokenService.RefreshTokenAsync(refreshToken);
                return new RefreshTokenResponse 
                {
                    AccessToken = tokenResponse.AccessToken, 
                    RefreshToken = tokenResponse.RefreshToken
                };
            }
            catch
            {
                return null;
            }
        }

        private bool IsPublicPath(string path)
        {
            var publicPaths = new[] { "/api/auth/login", "/api/auth/register", "/api/auth/google-login" };
            return publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
