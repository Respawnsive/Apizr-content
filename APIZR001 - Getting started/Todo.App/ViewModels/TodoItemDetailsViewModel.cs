using Todo.App.Models;

namespace Todo.App.ViewModels;

[QueryProperty(nameof(TodoItem), "TodoItem")]
public partial class TodoItemDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    TodoItem _todoItem;
}
