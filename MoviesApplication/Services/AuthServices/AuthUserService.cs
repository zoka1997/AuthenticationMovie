using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApplication.Authentication;
using MoviesApplication.Models.IdentityModels;
using MoviesApplication.Models.MessageModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MoviesApplication.Services.AuthServices
{
    public class AuthUserService : IAuthUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthUserService(UserManager<ApplicationUser> userManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            this.signInManager = signInManager;
        }

        public async Task<object> Register(RegisterUser model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return new Response { Status = "Error", Message = "Username already exists!" };
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                Salt = salt, // Store the salt in the user record
                PasswordHash = passwordHash // Store the hashed password in the user record
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return new Response { Status = "Error", Message = "User Creation failed!" };
            }

            return new Response { Status = "Success", Message = "User Created Successfully" };
        }

        public async Task<object> Login(LoginUser model, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = CreateToken(authClaims);

                if (rememberMe)
                {
                    // If the user selected "Remember Me", set a persistent cookie with the token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var serializedToken = tokenHandler.WriteToken(token);
                    return new
                    {
                        Token = serializedToken,
                        Expiration = token.ValidTo
                    };
                }

                var refreshToken = GenerateRefreshToken();
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                await _userManager.UpdateAsync(user);

                return new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };
            }

            return null;
        }

        public async Task<object> RegisterAdmin(RegisterUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return new Response { Status = "Error", Message = "User already exists!" };
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            return new Response { Status = "Success", Message = "User created successfully!" };
        }

        public async Task<Response> Logout()
        {
            await signInManager.SignOutAsync();

            var response = new Response
            {
                Message = "Logout success!",
                Status = "Success"
            };
            return response;
        }
    }
}