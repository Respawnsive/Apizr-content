using Todo.App.Models;

namespace Todo.App.ViewModels;

[QueryProperty(nameof(TodoItem), "TodoItem")]
public partial class TodoItemEditViewModel : BaseViewModel
{
    [ObservableProperty]
    TodoItem _todoItem;

    [RelayCommand]
    private async Task SaveAsync()
    {
    }
}
