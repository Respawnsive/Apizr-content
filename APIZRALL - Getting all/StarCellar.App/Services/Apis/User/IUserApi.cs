using Refit;
using StarCellar.App.Services.Apis.User.Dtos;
using Apizr.Configuring.Request;

namespace StarCellar.App.Services.Apis.User
{
    public interface IUserApi
    {
        [Post("signup")]
        Task<UserDTO> SignUpAsync(SignUpRequest request, [RequestOptions] IApizrRequestOptions options);

        [Post("signin")]
        Task<string> SignInAsync(SignInRequest request, [RequestOptions] IApizrRequestOptions options);

        [Post("refresh")]
        Task<string> RefreshAsync([RequestOptions] IApizrRequestOptions options);

        [Get("user/{id}")]
        Task GetProfileAsync(string id, [RequestOptions] IApizrRequestOptions options);

        [Delete("signout")]
        Task SignOutAsync([RequestOptions] IApizrRequestOptions options);
    }
}
