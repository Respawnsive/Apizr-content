using Apizr;
using Todo.App.Models;
using Todo.App.Services;
using Todo.App.Views;

namespace Todo.App.ViewModels;

public partial class TodoItemsViewModel : BaseViewModel
{
    private readonly IApizrManager<ITodoItemsApi> _todoItemsManager;
    private readonly IConnectivity _connectivity;

    public TodoItemsViewModel(IApizrManager<ITodoItemsApi> todoItemsManager, IConnectivity connectivity)
    {
        Title = "Todo items";

        _todoItemsManager = todoItemsManager;
        _connectivity = connectivity;
    }

    public ObservableCollection<TodoItem> TodoItems { get; } = new();

    [ObservableProperty] private bool _isRefreshing;

    [RelayCommand]
    private async Task GetTodoItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            var todoItems = await _todoItemsManager.ExecuteAsync(api => api.GetTodoItemsAsync());

            if(TodoItems.Count != 0)
                TodoItems.Clear();

            foreach(var todoItem in todoItems)
                TodoItems.Add(todoItem);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get TodoItems: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(TodoItemEditPage)}", true, new Dictionary<string, object>
        {
            {"TodoItem", new TodoItem() }
        });
    }

    [RelayCommand]
    private async Task GoToDetailsAsync(TodoItem todoItem)
    {
        if (todoItem == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(TodoItemDetailsPage)}", true, new Dictionary<string, object>
        {
            {"TodoItem", todoItem }
        });
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await GetTodoItemsAsync();
    }
}
