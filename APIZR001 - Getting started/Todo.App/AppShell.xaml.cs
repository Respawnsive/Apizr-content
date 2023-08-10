using Todo.App.Views;

namespace Todo.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(TodoItemDetailsPage), typeof(TodoItemDetailsPage));
        Routing.RegisterRoute(nameof(TodoItemEditPage), typeof(TodoItemEditPage));
}
}