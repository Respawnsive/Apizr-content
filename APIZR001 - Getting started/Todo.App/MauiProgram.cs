using Apizr;
using Microsoft.Extensions.Logging;
using Todo.App.Services;
using Todo.App.Views;

namespace Todo.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Plugins
    	builder.Services.AddSingleton(Connectivity.Current);

        // Services
        builder.Services.AddApizrManagerFor<ITodoItemsApi>();

        // Presentation
        builder.Services.AddSingleton<TodoItemsViewModel>();
		builder.Services.AddSingleton<TodoItemsPage>();

		builder.Services.AddTransient<TodoItemDetailsViewModel>();
		builder.Services.AddTransient<TodoItemDetailsPage>();

		return builder.Build();
	}
}
