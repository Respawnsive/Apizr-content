namespace StarCellar.App.Services.Apis.Cellar.Dtos
{
    public class Wine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Score { get; set; }
        public Guid OwnerId { get; set; }
    }
}
