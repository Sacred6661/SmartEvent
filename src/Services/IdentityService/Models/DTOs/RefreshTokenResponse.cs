namespace IdentityService.Models.DTOs
{
    public class RefreshTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
