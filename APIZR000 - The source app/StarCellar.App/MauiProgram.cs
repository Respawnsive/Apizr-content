using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Files;
using StarCellar.App.Services.Apis.User;
using StarCellar.App.Services.Apis.User.Dtos;
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
            .AddSingleton(SecureStorage.Default);

        builder.Services.AddRefitClient<IUserApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7015"));

        builder.Services.AddRefitClient<ICellarApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7015"));

        builder.Services.AddRefitClient<IFileApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
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
