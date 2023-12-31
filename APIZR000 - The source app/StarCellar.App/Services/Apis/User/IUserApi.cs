﻿using Refit;
using StarCellar.App.Services.Apis.User.Dtos;

namespace StarCellar.App.Services.Apis.User
{
    public interface IUserApi
    {
        [Post("/signup")]
        Task<Dtos.User> SignUpAsync(SignUpRequest request);

        [Post("/signin")]
        Task<Tokens> SignInAsync(SignInRequest request);

        [Post("/refresh")]
        Task<Tokens> RefreshAsync(Tokens tokens);

        [Get("/profile")]
        [Headers("Authorization: Bearer")]
        Task<Dtos.User> GetProfileAsync();

        [Delete("/signout")]
        Task SignOutAsync(Tokens tokens);
    }
}
