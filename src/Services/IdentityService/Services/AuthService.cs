using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Models.DTOs;
using IdentityService.Services.Interfaces;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using SmartEvent.Shared.Contracts.Events;
using System.Security.Claims;

namespace IdentityService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            IdentityDbContext context,
            IPublishEndpoint publishEndpoint,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Registering new user with email: {Email}", request.Email);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponse { Success = false, Message = "User with this email already exists" };
            }

            var user = request.Adapt<ApplicationUser>();
            user.UserName = request.Email;
            user.CreatedAt = DateTime.UtcNow;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                return new AuthResponse { Success = false, Message = errors };
            }

            // publish event
            await _publishEndpoint.Publish(new UserCreatedEvent
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = DateTime.UtcNow.ToString("O")
            });

            // generate token for auto-login
            var tokenResponse = await _tokenService.GenerateTokensAsync(user);

            _logger.LogInformation("User created and logged in successfully: {UserId}", user.Id);

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Data = user.Adapt<UserInfo>(),
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "Invalid email or password" };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponse { Success = false, Message = "Invalid email or password" };
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Token generating
            var tokenResponse = await _tokenService.GenerateTokensAsync(user);

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Data = user.Adapt<UserInfo>(),
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }

        public async Task<AuthResponse> GoogleLoginAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return new AuthResponse { Success = false, Message = "No HTTP context" };

            var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return new AuthResponse { Success = false, Message = "Google authentication failed" };

            var claims = result.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "";
            var lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? "";
            var avatarUrl = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                return new AuthResponse { Success = false, Message = "Invalid Google response" };

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    GoogleId = googleId,
                    AvatarUrl = avatarUrl,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user from Google: {Errors}", errors);
                    return new AuthResponse { Success = false, Message = errors };
                }

                await _publishEndpoint.Publish(new UserCreatedEvent
                {
                    UserId = user.Id.ToString(),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = DateTime.UtcNow.ToString("O")
                });
            }
            else
            {
                user.GoogleId = googleId;
                user.AvatarUrl = avatarUrl ?? user.AvatarUrl;
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            // token generating
            var tokenResponse = await _tokenService.GenerateTokensAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Google login successful",
                Data = user.Adapt<UserInfo>(),
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }

        public async Task<UserInfo?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Adapt<UserInfo>();
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;

            return await GetUserByIdAsync(Guid.Parse(userId));
        }
    }
}