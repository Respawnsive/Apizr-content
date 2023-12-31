﻿using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Cellar.Dtos;
using StarCellar.App.Services.Navigation;
using StarCellar.App.Views;

namespace StarCellar.App.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineDetailsViewModel : BaseViewModel
{
    private readonly ICellarApi _cellarApi;
    private readonly IConnectivity _connectivity;

    public WineDetailsViewModel(INavigationService navigationService, 
        ICellarApi cellarApi, 
        IConnectivity connectivity) : base(navigationService)
    {
        _cellarApi = cellarApi;
        _connectivity = connectivity;
    }

    [ObservableProperty]
    Wine _wine;

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await NavigationService.GoToAsync(nameof(WineEditPage), true, new Dictionary<string, object>
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

            var confirm = await NavigationService.DisplayAlert("Delete?",
                $"Please confirm you really want to delete it.", "Confirm", "Cancel");
            if(!confirm)
                return;

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await NavigationService.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            await _cellarApi.DeleteWineAsync(Wine.Id);

            await NavigationService.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to delete Wine: {ex.Message}");
            await NavigationService.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

    }
}
