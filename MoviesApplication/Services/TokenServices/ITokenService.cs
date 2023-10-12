using Microsoft.AspNetCore.Mvc;
using MoviesApplication.Models.TokenModels;
using System.Security.Claims;

namespace MoviesApplication.Services.TokenServices
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateToken(List<Claim> authClaims);
        Task<string> GenerateRefreshToken();
        Task<object> RefreshToken(TokenUser tokenModel);
        Task RevokeAllRefreshTokens();
        Task<IActionResult> RevokeToken(string username);
        Task<bool> ValidateToken(string token);
    }
}