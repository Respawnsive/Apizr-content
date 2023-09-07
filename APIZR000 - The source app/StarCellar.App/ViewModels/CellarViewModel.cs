using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Cellar.Dtos;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

public partial class CellarViewModel : BaseViewModel
{
    private readonly ICellarApi _cellarApi;
    private readonly IConnectivity _connectivity;

    public CellarViewModel(ICellarApi cellarApi, IConnectivity connectivity)
    {
        _cellarApi = cellarApi;
        _connectivity = connectivity;
    }

    public ObservableCollection<Wine> Wines { get; } = new();

    [ObservableProperty] private bool _isRefreshing;

    [RelayCommand]
    private async Task GetWinesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            var wines = await _cellarApi.GetWinesAsync();

            if(Wines.Count != 0)
                Wines.Clear();

            foreach(var wine in wines)
                Wines.Add(wine);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get Wines: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(WineEditPage)}", true, new Dictionary<string, object>
        {
            {nameof(Wine), new Wine() }
        });
    }

    [RelayCommand]
    private async Task GoToDetailsAsync(Wine wine)
    {
        if (wine == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(WineDetailsPage)}", true, new Dictionary<string, object>
        {
            {nameof(Wine), wine }
        });
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await GetWinesAsync();
    }
}
