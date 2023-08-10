namespace StarCellar.Api
{
    public class Wine
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Score { get; set; }
        public string? Secret { get; set; }
    }
}