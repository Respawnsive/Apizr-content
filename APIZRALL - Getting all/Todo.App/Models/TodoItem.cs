namespace Todo.App.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public string ImageUrl { get; set; }
    public bool IsComplete { get; set; }
}