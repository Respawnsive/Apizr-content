namespace Todo.App.Views;

public partial class TodoItemsPage : ContentPage
{
	public TodoItemsPage(TodoItemsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

