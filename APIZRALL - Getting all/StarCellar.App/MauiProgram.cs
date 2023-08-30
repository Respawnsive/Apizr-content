using Apizr;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using StarCellar.App.Services;
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
		builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

		// Plugins
    	builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddSingleton(FilePicker.Default);

        // Services
        builder.Services.AddApizr(registry => 
            registry.AddManagerFor<ICellarApi>()
                .AddUploadManagerWith<string>(options => options.WithBasePath("upload")),
            options => options.WithBaseAddress("https://localhost:7015"));

        // Presentation
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddSingleton<CellarViewModel>();
		builder.Services.AddSingleton<CellarPage>();

		builder.Services.AddTransient<WineDetailsViewModel>();
		builder.Services.AddTransient<WineDetailsPage>();

        builder.Services.AddTransient<WineEditViewModel>();
        builder.Services.AddTransient<WineEditPage>();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        return builder.Build();
	}
}
