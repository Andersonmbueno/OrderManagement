using OrderManagement.Api.Models;

namespace OrderManagement.Api.Authentication;

public interface IJwtTokenService
{
    LoginResponse CreateToken(string username);
}
