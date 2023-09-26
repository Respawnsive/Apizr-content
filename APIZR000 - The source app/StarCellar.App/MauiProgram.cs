using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Files;
using StarCellar.App.Services.Apis.User;
using StarCellar.App.Services.Apis.User.Dtos;
using StarCellar.App.Services.Navigation;
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
            .UseUraniumUIBlurs()
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
            .AddSingleton(SecureStorage.Default)
            .AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddRefitClient<IUserApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://hgksj1pl-7015.uks1.devtunnels.ms"));

        builder.Services.AddRefitClient<ICellarApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://hgksj1pl-7015.uks1.devtunnels.ms"));

        builder.Services.AddRefitClient<IFileApi>(serviceProvider => new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, _) =>
                    serviceProvider.GetRequiredService<ISecureStorage>().GetAsync(nameof(Tokens.AccessToken))
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://hgksj1pl-7015.uks1.devtunnels.ms"));

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
