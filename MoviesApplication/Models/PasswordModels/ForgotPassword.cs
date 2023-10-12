using System.ComponentModel.DataAnnotations;

namespace MoviesApplication.Models.PasswordModels
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
