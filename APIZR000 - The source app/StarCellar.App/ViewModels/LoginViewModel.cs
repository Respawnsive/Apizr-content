using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Networking;
using MiniValidation;
using StarCellar.App.Services.Apis.User;
using StarCellar.App.Services.Apis.User.Dtos;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IUserApi _userApi;
    private readonly IConnectivity _connectivity;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;

    public LoginViewModel(IUserApi userApi, IConnectivity connectivity)
    {
        _userApi = userApi;
        _connectivity = connectivity;
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

            var token = await _userApi.SignInAsync(signInRequest);
            if (string.IsNullOrEmpty(token))
            {
                await Toast.Make("Unable to signin, please try again later.", ToastDuration.Long).Show();
                return;
            }

            Preferences.Default.Set("token", token.Access_Token);
            Preferences.Default.Set("refreshToken", token.Refresh_Token);

            await Shell.Current.GoToAsync($"//{nameof(CellarPage)}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
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