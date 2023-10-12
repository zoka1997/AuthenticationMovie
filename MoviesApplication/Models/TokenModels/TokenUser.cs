namespace MoviesApplication.Models.TokenModels
{
    public class TokenUser
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set;} = string.Empty;
    }
}
