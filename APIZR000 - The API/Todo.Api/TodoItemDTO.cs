namespace Todo.Api
{
    public class TodoItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public bool IsComplete { get; set; }

        public TodoItemDTO() { }
        public TodoItemDTO(Todo todoItem) =>
            (Id, Name, Notes, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.Notes, todoItem.IsComplete);
    }
}
