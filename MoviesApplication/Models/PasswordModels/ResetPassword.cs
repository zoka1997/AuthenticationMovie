using System.ComponentModel.DataAnnotations;

namespace MoviesApplication.Models.PasswordModels
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
