using Apizr;
using StarCellar.App.Models;
using StarCellar.App.Services;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

public partial class CellarViewModel : BaseViewModel
{
    private readonly IApizrManager<ICellarApi> _cellarManager;
    private readonly IConnectivity _connectivity;

    public CellarViewModel(IApizrManager<ICellarApi> cellarManager, IConnectivity connectivity)
    {
        _cellarManager = cellarManager;
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

            var wines = await _cellarManager.ExecuteAsync(api => api.GetWinesAsync());

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
