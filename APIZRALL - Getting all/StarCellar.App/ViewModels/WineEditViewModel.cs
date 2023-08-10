using Apizr;
using StarCellar.App.Models;
using StarCellar.App.Services;

namespace StarCellar.App.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineEditViewModel : BaseViewModel
{
    private readonly IApizrManager<ICellarApi> _cellarManager;
    private readonly IConnectivity _connectivity;

    public WineEditViewModel(IApizrManager<ICellarApi> cellarManager, IConnectivity connectivity)
    {
        _cellarManager = cellarManager;
        _connectivity = connectivity;
    }

    [ObservableProperty] Wine _wine;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if(string.IsNullOrWhiteSpace(Wine.Name))
            {
                await Shell.Current.DisplayAlert("Name required!",
                    $"Please give it a name and try again.", "OK");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            if (Wine.Id <= 0)
                Wine = await _cellarManager.ExecuteAsync(api => api.CreateWineAsync(Wine));
            else
                await _cellarManager.ExecuteAsync(api => api.UpdateWineAsync(Wine.Id, Wine));

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to save Wine: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
