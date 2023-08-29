using System.ComponentModel.DataAnnotations;

namespace StarCellar.Api.Data
{
    public record Token
    {
        [Key]
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
    }
}
