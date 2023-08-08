using Apizr;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
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
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
#if DEBUG
		builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
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

        builder.Services.AddTransient<TodoItemEditViewModel>();
        builder.Services.AddTransient<TodoItemEditPage>();

return builder.Build();
	}
}
