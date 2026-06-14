using IdentityService.Models.DTOs;

namespace IdentityService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> GoogleLoginAsync();
        Task<UserInfo?> GetUserByIdAsync(Guid userId);
        Task<UserInfo?> GetCurrentUserAsync();
    }
}
