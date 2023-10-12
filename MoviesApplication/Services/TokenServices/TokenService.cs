using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesApplication.Authentication;
using MoviesApplication.Models.TokenModels;
using MoviesApplication.Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MoviesApplication.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IAuthUserService _authUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IAuthUserService authUserService, UserManager<ApplicationUser> userManager)
        {
            _authUserService = authUserService;
            _userManager = userManager;

        }

        public async Task<object> RefreshToken(TokenUser tokenModel)
        {
            if (tokenModel is null || tokenModel.AccessToken is null || tokenModel.RefreshToken is null)
            {
                return BadRequestResponse("Invalid client request");
            }

            var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null)
            {
                return BadRequestResponse("Invalid access token or refresh token");
            }

            string? username = principal.Identity?.Name;

            if (username is null)
            {
                return BadRequestResponse("Invalid access token or refresh token");
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequestResponse("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };
        }

        private object BadRequestResponse(string message)
        {
            return new ObjectResult(new { Message = message })
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public async Task<IActionResult> RevokeToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new BadRequestObjectResult("Invalid user name");
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return new NoContentResult();
        }

        public async Task<TokenResponse> CreateToken(List<Claim> authClaims)
        {
            var tokenValidityInMinutes = int.Parse(_configuration["JWT:TokenValidityInMinutes"]);
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = await GenerateRefreshToken();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<bool> ValidateToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                ValidateLifetime = false
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                return securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public async Task RevokeAllRefreshTokens()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
