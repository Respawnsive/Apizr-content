using Refit;
using StarCellar.App.Services.Apis.User.Dtos;

namespace StarCellar.App.Services.Apis.User
{
    public interface IUserApi
    {
        [Post("signup")]
        Task<Dtos.User> SignUpAsync(SignUpRequest request);

        [Post("signin")]
        Task<string> SignInAsync(SignInRequest request);

        [Post("refresh")]
        Task<string> RefreshAsync();

        [Get("user/{id}")]
        Task GetProfileAsync(string id);

        [Delete("signout")]
        Task SignOutAsync();
    }
}
