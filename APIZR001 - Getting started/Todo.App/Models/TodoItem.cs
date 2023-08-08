using System.Text.Json.Serialization;

namespace Todo.App.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public bool IsComplete { get; set; }
}

//[JsonSerializable(typeof(List<TodoItem>))]
//internal sealed partial class TodoItemContext : JsonSerializerContext{

//}