namespace StarCellar.Api
{
    public class WineDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Score { get; set; }

        public WineDTO() { }
        public WineDTO(Wine wine) =>
            (Id, Name, Description, ImageUrl, Stock, Score) = (wine.Id, wine.Name, wine.Description, wine.ImageUrl, wine.Stock, wine.Score);
    }
}
