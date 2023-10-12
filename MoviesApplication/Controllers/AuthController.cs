using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApplication.Authentication;
using MoviesApplication.Models.IdentityModels;
using MoviesApplication.Models.MessageModels;
using MoviesApplication.Services.AuthServices;
using MoviesApplication.Services.TokenServices;

namespace MoviesApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IAuthUserService _authUserService;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ITokenService tokenService, IAuthUserService authUserService)
        {
            this.signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _authUserService = authUserService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var result = await _authUserService.Register(model);
            if (result is Response response)
            {
                if (response.Status == "Success")
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            return BadRequest("Invalid response from registration service.");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser model, bool rememberMe = false)
        {
            var result = await _authUserService.Login(model, rememberMe);
            if (result != null)
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUser model)
        {
            var result = await _authUserService.RegisterAdmin(model);
            if (result is Response response)
            {
                if (response.Status == "Success")
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            return BadRequest("Invalid response from registration service.");
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _authUserService.Logout();
            return Ok(response);
        }
    }
}
