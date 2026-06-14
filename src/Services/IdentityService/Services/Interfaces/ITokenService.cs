using IdentityService.Models;
using IdentityService.Models.DTOs;

namespace IdentityService.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(Guid userId);
    }
}
