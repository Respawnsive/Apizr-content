namespace StarCellar.App.Services.Apis.Cellar.Dtos
{
    public record WineDTO(Guid Id, string Name, string Description, string ImageUrl, int Stock, int Score,
        Guid OwnerId);
}
