using Apizr;
using Todo.App.Models;
using Todo.App.Services;

namespace Todo.App.ViewModels;

[QueryProperty(nameof(TodoItem), "TodoItem")]
public partial class TodoItemEditViewModel : BaseViewModel
{
    private readonly IApizrManager<ITodoItemsApi> _todoItemsManager;
    private readonly IConnectivity _connectivity;

    public TodoItemEditViewModel(IApizrManager<ITodoItemsApi> todoItemsManager, IConnectivity connectivity)
    {
        _todoItemsManager = todoItemsManager;
        _connectivity = connectivity;
    }

    [ObservableProperty] TodoItem _todoItem;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if(string.IsNullOrWhiteSpace(TodoItem.Name))
            {
                await Shell.Current.DisplayAlert("Name required!",
                    $"Please give it a name and try again.", "OK");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            if (TodoItem.Id <= 0)
                TodoItem = await _todoItemsManager.ExecuteAsync(api => api.CreateTodoItemAsync(TodoItem));
            else
                await _todoItemsManager.ExecuteAsync(api => api.UpdateTodoItemAsync(TodoItem.Id, TodoItem));

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to save TodoItem: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
