using System.ComponentModel.DataAnnotations;

namespace StarCellar.App.Services.Apis.User.Dtos
{
    public record SignInRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
