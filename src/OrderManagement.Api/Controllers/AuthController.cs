using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Authentication;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(IJwtTokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var configuredUsername = _configuration["Authentication:Username"];
        var configuredPassword = _configuration["Authentication:Password"];

        if (!string.Equals(request.Username, configuredUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, configuredPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        return Ok(_tokenService.CreateToken(request.Username));
    }
}
