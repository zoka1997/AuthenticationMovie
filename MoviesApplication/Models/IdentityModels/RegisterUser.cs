using System.ComponentModel.DataAnnotations;

namespace MoviesApplication.Models.IdentityModels
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirmation Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
