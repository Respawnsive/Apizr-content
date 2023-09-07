using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Cellar.Dtos;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineDetailsViewModel : BaseViewModel
{
    private readonly ICellarApi _cellarApi;
    private readonly IConnectivity _connectivity;

    public WineDetailsViewModel(ICellarApi cellarApi, IConnectivity connectivity)
    {
        _cellarApi = cellarApi;
        _connectivity = connectivity;
    }

    [ObservableProperty]
    Wine _wine;

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await Shell.Current.GoToAsync(nameof(WineEditPage), true, new Dictionary<string, object>
        {
            {nameof(Wine), Wine }
        });
    }

    [RelayCommand]
    private async Task DeleteWineAsync()
    {
        if (IsBusy)
            return;

        try
        {

            var confirm = await Shell.Current.DisplayAlert("Delete?",
                $"Please confirm you really want to delete it.", "Confirm", "Cancel");
            if(!confirm)
                return;

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            await _cellarApi.DeleteWineAsync(Wine.Id);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to delete Wine: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

    }
}
