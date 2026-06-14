using FluentValidation;
using IdentityService.Models.DTOs;
using IdentityService.Services;
using IdentityService.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEvent.Shared.Abstractions.Extensions;
using SmartEvent.Shared.Abstractions.Models;
using System.Security.Claims;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        ILogger<AuthController> logger,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        IConfiguration configuration)
    {
        _authService = authService;
        _tokenService = tokenService;
        _logger = logger;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var validationError = await _registerValidator.ValidateAndRespond(request);
        if (validationError != null)
            return BadRequest(validationError);

        var response = await _authService.RegisterAsync(request);

        if (!response.Success)
            return BadRequest(response);

        // Setting tokens in cookies
        SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
            .SetTokenCookies(Response, response.AccessToken!, response.RefreshToken!);

        // We return only user information
        return Ok(new ApiResponse<UserInfo>
        {
            Success = true,
            Message = response.Message,
            Data = response.Data
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationError = await _loginValidator.ValidateAndRespond(request);
        if (validationError != null)
            return BadRequest(validationError);

        var response = await _authService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        // set access token and refresh token to http only cookies
        SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
            .SetTokenCookies(Response, response.AccessToken!, response.RefreshToken!);

        // return only user Info
        return Ok(new ApiResponse<UserInfo>
        {
            Success = true,
            Message = "Login successful",
            Data = response.Data
        });
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "";

        try
        {
            var response = await _authService.GoogleLoginAsync();

            if (!response.Success)
                return Redirect($"{frontendUrl}/login?error=google_auth_failed");

            // set tokens to cookies
            SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
                .SetTokenCookies(Response, response.AccessToken!, response.RefreshToken!);

            return Redirect($"{frontendUrl}/auth/callback");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google callback");
            return Redirect($"{frontendUrl}/login?error=google_auth_failed");
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(ApiResponse.Fail("Refresh token not found"));

        try
        {
            var tokenResponse = await _tokenService.RefreshTokenAsync(refreshToken);

            // refresh refreshToken cookie with accesToken
            SmartEvent.Shared.Abstractions.Extensions.CookieExtensions
                 .SetTokenCookies(Response, tokenResponse.AccessToken!, tokenResponse.RefreshToken!);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = tokenResponse.AccessToken
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Unauthorized(ApiResponse.Fail("Invalid refresh token"));
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _tokenService.RevokeTokenAsync(Guid.Parse(userId));
        SmartEvent.Shared.Abstractions.Extensions.CookieExtensions.ClearTokenCookies(Response);

        return Ok(ApiResponse.Ok("Logged out successfully"));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _authService.GetCurrentUserAsync();

        return user == null
            ? Unauthorized(new ApiResponse<UserInfo> { Success = false, Message = "User not authenticated" })
            : Ok(new ApiResponse<UserInfo> { Success = true, Data = user });
    }

    [HttpGet("me/{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);

        return user == null
            ? NotFound(ApiResponse<UserInfo>.Fail("User not found"))
            : Ok(ApiResponse<UserInfo>.Ok(user));
    }
}