using System.ComponentModel.DataAnnotations;

namespace MoviesApplication.Models.IdentityModels
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
