using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MiniValidation;
using StarCellar.App.Services.Apis.User;
using StarCellar.App.Services.Apis.User.Dtos;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IUserApi _userApi;
    private readonly IConnectivity _connectivity;
    private readonly ISecureStorage _secureStorage;
    [ObservableProperty] private bool _isSigned;
    [ObservableProperty] private bool _isInitialized;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;

    public LoginViewModel(IUserApi userApi, IConnectivity connectivity, ISecureStorage secureStorage)
    {
        _userApi = userApi;
        _connectivity = connectivity;
        _secureStorage = secureStorage;
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        if (IsBusy || IsInitialized)
            return;

        try
        {
            IsBusy = true;

            var accessToken = await _secureStorage.GetAsync(nameof(Tokens.AccessToken));
            var refreshToken = await _secureStorage.GetAsync(nameof(Tokens.RefreshToken));
            IsSigned = !string.IsNullOrWhiteSpace(accessToken) && !string.IsNullOrWhiteSpace(refreshToken);
            if (IsSigned)
            {
                var tokens = new Tokens(accessToken, refreshToken);
                tokens = await _userApi.RefreshAsync(tokens);

                if (!string.IsNullOrWhiteSpace(tokens.AccessToken) && !string.IsNullOrWhiteSpace(tokens.RefreshToken))
                {
                    await _secureStorage.SetAsync(nameof(Tokens.AccessToken), tokens.AccessToken);
                    await _secureStorage.SetAsync(nameof(Tokens.RefreshToken), tokens.RefreshToken);

                    await Shell.Current.GoToAsync($"//{nameof(CellarPage)}");
                }
            }
            else
            {
                IsInitialized = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to initialize: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsBusy)
            return;

        // Connectivity
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No connectivity!",
                $"Please check internet and try again.", "OK");
            return;
        }

        // Validation
        var signInRequest = new SignInRequest()
        {
            Login = Email,
            Password = Password
        };

        if (!MiniValidator.TryValidate(signInRequest, out var errors))
        {
            var sb = new StringBuilder();
            foreach (var entry in errors)
            {
                sb.Append($"{entry.Key}:\n");
                foreach (var error in entry.Value) 
                    sb.Append($"  - {error}\n");
            }

            await Shell.Current.DisplayAlert("Input error!", sb.ToString(), "OK");
            return;
        }

        // Requesting
        try
        {
            IsBusy = true;

            var tokens = await _userApi.SignInAsync(signInRequest);
            if (string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrWhiteSpace(tokens.RefreshToken))
            {
                await Toast.Make("Unable to signin, please try again later.", ToastDuration.Long).Show();
                return;
            }

            await _secureStorage.SetAsync(nameof(Tokens.AccessToken), tokens.AccessToken);
            await _secureStorage.SetAsync(nameof(Tokens.RefreshToken), tokens.RefreshToken);

            await Shell.Current.GoToAsync($"//{nameof(CellarPage)}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to signin: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}