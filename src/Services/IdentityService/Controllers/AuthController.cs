using FluentValidation;
using IdentityService.Models.DTOs;
using IdentityService.Services;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using SmartEvent.Shared.Abstractions.Extensions;
using SmartEvent.Shared.Abstractions.Models;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _logger = logger;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Validation
        var validationError = await _registerValidator.ValidateAndRespond(request);
        if (validationError != null)
            return BadRequest(validationError);

        var response = await _authService.RegisterAsync(request);
        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationError = await _loginValidator.ValidateAndRespond(request);
        if (validationError != null)
            return BadRequest(validationError);

        var response = await _authService.LoginAsync(request);
        return response.Success
            ? Ok(response)
            : Unauthorized(response);
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var response = await _authService.GoogleLoginAsync();

        if (!response.Success)
            return BadRequest(response);

        return Redirect($"http://localhost:3000/auth/callback?token={response.Token}");
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