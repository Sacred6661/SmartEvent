using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SmartEvent.Shared.Abstractions.Extensions
{
    public static class CookieExtensions
    {
        public static void SetTokenCookies(this HttpResponse response, string accessToken, string refreshToken)
        {
            response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Path = "/"
            });

            response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            });
        }

        public static void ClearTokenCookies(this HttpResponse response)
        {
            response.Cookies.Delete("accessToken");
            response.Cookies.Delete("refreshToken");
        }

        public static string? GetAccessToken(this HttpRequest request)
        {
            return request.Cookies["accessToken"];
        }

        public static string? GetRefreshToken(this HttpRequest request)
        {
            return request.Cookies["refreshToken"];
        }
    }
}
