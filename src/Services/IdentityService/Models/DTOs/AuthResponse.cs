using SmartEvent.Shared.Abstractions.Models;

namespace IdentityService.Models.DTOs
{
    public class AuthResponse : ApiResponse<UserInfo>
    {
        public string? Token { get; set; }
    }
}
