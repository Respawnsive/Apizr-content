using Apizr;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using StarCellar.App.Services;
using StarCellar.App.ViewModels;
using StarCellar.App.Views;

namespace StarCellar.App;

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
        builder.Services.AddSingleton(FilePicker.Default);

        // Services
        builder.Services.AddApizr(registry => 
            registry.AddManagerFor<ICellarApi>()
                .AddUploadManagerWith<string>(options => options.WithBasePath("upload")),
            options => options.WithBaseAddress("https://localhost:7015"));

        // Presentation
        builder.Services.AddSingleton<CellarViewModel>();
		builder.Services.AddSingleton<CellarPage>();

		builder.Services.AddTransient<WineDetailsViewModel>();
		builder.Services.AddTransient<WineDetailsPage>();

        builder.Services.AddTransient<WineEditViewModel>();
        builder.Services.AddTransient<WineEditPage>();

        return builder.Build();
	}
}
