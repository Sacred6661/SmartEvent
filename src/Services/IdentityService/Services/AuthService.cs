using IdentityService.Models;
using IdentityService.Models.DTOs;
using IdentityService.Services.Interfaces;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using SmartEvent.Shared.Contracts.Events;

namespace IdentityService.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IPublishEndpoint publishEndpoint,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

        _logger.LogInformation("User created successfully: {UserId}", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "User registered successfully",
            Data = user.Adapt<UserInfo>()
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

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Data = user.Adapt<UserInfo>()
        };
    }

    public async Task<AuthResponse> GoogleLoginAsync()
    {
        // TODO: need realization after google OAuth config
        throw new NotImplementedException();
    }

    public async Task<UserInfo?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.Adapt<UserInfo>();
    }
}