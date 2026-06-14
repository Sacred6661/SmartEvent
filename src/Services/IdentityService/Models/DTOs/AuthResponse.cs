using SmartEvent.Shared.Abstractions.Models;

namespace IdentityService.Models.DTOs
{
    public class AuthResponse : ApiResponse<UserInfo>
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
