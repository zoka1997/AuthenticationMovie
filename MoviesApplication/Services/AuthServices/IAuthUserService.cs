using MoviesApplication.Models.IdentityModels;
using MoviesApplication.Models.MessageModels;

namespace MoviesApplication.Services.AuthServices
{
    public interface IAuthUserService
    {
        Task<object> Login(LoginUser model, bool rememberMe);
        Task<object> Register(RegisterUser model);
        Task<object> RegisterAdmin(RegisterUser model);
        Task<Response> Logout();
    }
}