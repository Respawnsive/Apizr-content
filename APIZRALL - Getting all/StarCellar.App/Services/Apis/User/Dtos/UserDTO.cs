using System.ComponentModel.DataAnnotations;

namespace StarCellar.App.Services.Apis.User.Dtos
{
    public record UserDTO
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public int Age { get; set; }

        [Required]
        public string Role { get; set; }

        public string Address { get; set; }
    }
}
