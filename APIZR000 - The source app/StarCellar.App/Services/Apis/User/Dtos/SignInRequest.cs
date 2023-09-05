﻿using System.ComponentModel.DataAnnotations;

namespace StarCellar.App.Services.Apis.User.Dtos
{
    public record SignInRequest
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; }
    }
}
