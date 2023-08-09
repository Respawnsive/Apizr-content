using Apizr;
using Todo.App.Models;
using Todo.App.Services;
using Todo.App.Views;

namespace Todo.App.ViewModels;

[QueryProperty(nameof(TodoItem), "TodoItem")]
public partial class TodoItemDetailsViewModel : BaseViewModel
{
    private readonly IApizrManager<ITodoItemsApi> _todoItemsManager;
    private readonly IConnectivity _connectivity;

    public TodoItemDetailsViewModel(IApizrManager<ITodoItemsApi> todoItemsManager, IConnectivity connectivity)
    {
        _todoItemsManager = todoItemsManager;
        _connectivity = connectivity;
    }

    [ObservableProperty]
    TodoItem _todoItem;

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await Shell.Current.GoToAsync(nameof(TodoItemEditPage), true, new Dictionary<string, object>
        {
            {"TodoItem", TodoItem }
        });
    }

    [RelayCommand]
    private async Task DeleteTodoItemAsync()
    {
        if (IsBusy)
            return;

        try
        {

            var confirm = await Shell.Current.DisplayAlert("Delete?",
                $"Please confirm you really want to delete it.", "Confirm", "Cancel");
            if(!confirm)
                return;

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            await _todoItemsManager.ExecuteAsync(api => api.DeleteTodoItemAsync(TodoItem.Id));

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get TodoItems: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

    }
}
