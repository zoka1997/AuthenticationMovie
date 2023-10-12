using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApplication.Authentication;
using MoviesApplication.Models.MessageModels;
using MoviesApplication.Models.PasswordModels;
using MoviesApplication.Services.EmailServices;

namespace MoviesApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSenderService;

        public EmailController(UserManager<ApplicationUser> userManager, IEmailSenderService emailSenderService, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
            {
                return Ok(); // Returning 200 OK even if the user is not found for security reasons
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);

            var message = new MessageEmail(new string[] { user.Email }, "Reset password token", callback, null!);
            await _emailSenderService.SendEmailAsync(message);

            return Ok();
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                return NotFound(); // User not found
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                return BadRequest(resetPassResult.Errors); // Returning the error messages
            }
            return Ok(); // Password reset successful
        }
    }
}
