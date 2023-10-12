using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApplication.Authentication;
using MoviesApplication.Models.TokenModels;
using MoviesApplication.Services.AuthServices;
using MoviesApplication.Services.TokenServices;

namespace MoviesApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly AuthUserService _authUserService;
        private readonly TokenService _tokenService;

        public TokenController(UserManager<ApplicationUser> userManager, IConfiguration configuration, AuthUserService authUserService, TokenService tokenService)
        {
            this.userManager = userManager;
            _configuration = configuration;
            _authUserService = authUserService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenUser tokenModel)
        {
            var result = await _authUserService.RefreshToken(tokenModel);
            if (result is ObjectResult objectResult)
            {
                return objectResult;
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var result = await _authUserService.RevokeToken(username);
            return result;
        }

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            await _authUserService.RevokeAllRefreshTokens();
            return NoContent();
        }
    }
}
