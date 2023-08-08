namespace Todo.App.Views;

public partial class TodoItemDetailsPage : ContentPage
{
	public TodoItemDetailsPage(TodoItemDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}