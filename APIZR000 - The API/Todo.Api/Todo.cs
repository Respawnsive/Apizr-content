namespace Todo.Api
{
    public class Todo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public bool IsComplete { get; set; }
        public string? Secret { get; set; }
    }
}