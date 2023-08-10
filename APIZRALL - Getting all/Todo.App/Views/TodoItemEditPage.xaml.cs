namespace Todo.App.Views;

public partial class TodoItemEditPage : ContentPage
{
	public TodoItemEditPage(TodoItemEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}