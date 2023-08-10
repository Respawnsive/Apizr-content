using Apizr;
using Cellar.app.Services;
using Cellar.app.ViewModels;
using Cellar.app.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Cellar.app;

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
        builder.Services.AddApizrManagerFor<ICellarApi>();

        // Presentation
        builder.Services.AddSingleton<WinesViewModel>();
		builder.Services.AddSingleton<WinesPage>();

		builder.Services.AddTransient<WineDetailsViewModel>();
		builder.Services.AddTransient<WineDetailsPage>();

        builder.Services.AddTransient<WineEditViewModel>();
        builder.Services.AddTransient<WineEditPage>();

        return builder.Build();
	}
}
