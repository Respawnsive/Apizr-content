using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.ViewModels;
using StarCellar.App.Views;
using UraniumUI;

namespace StarCellar.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialIconFonts();
            });
#if DEBUG
		builder.Logging.AddDebug()
            .SetMinimumLevel(LogLevel.Trace);
#endif

		// Infrastructure
    	builder.Services.AddSingleton(Connectivity.Current)
            .AddSingleton(FilePicker.Default)
            .AddRefitClient<ICellarApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7015"));

        // Presentation
        builder.Services.AddTransient<LoginViewModel>()
            .AddTransient<LoginPage>()
            .AddTransient<RegisterViewModel>()
            .AddTransient<RegisterPage>()
            .AddSingleton<CellarViewModel>()
            .AddSingleton<CellarPage>()
            .AddTransient<WineDetailsViewModel>()
            .AddTransient<WineDetailsPage>()
            .AddTransient<WineEditViewModel>()
            .AddTransient<WineEditPage>()
            .AddTransient<ProfileViewModel>()
            .AddTransient<ProfilePage>();

        return builder.Build();
	}
}
