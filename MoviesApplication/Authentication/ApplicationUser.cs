using Microsoft.AspNetCore.Identity;

namespace MoviesApplication.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        // Adding custom properties
        // Add a custom property for storing the salt
        public string Salt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
