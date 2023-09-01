using Refit;
using StarCellar.App.Services.Apis.Cellar;
using StarCellar.App.Services.Apis.Cellar.Dtos;

namespace StarCellar.App.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineEditViewModel : BaseViewModel
{
    private readonly ICellarApi _cellarApi;
    private readonly IConnectivity _connectivity;
    private readonly IFilePicker _filePicker;

    public WineEditViewModel(ICellarApi cellarApi,
        IConnectivity connectivity,
        IFilePicker filePicker)
    {
        _cellarApi = cellarApi;
        _connectivity = connectivity;
        _filePicker = filePicker;
    }

    [ObservableProperty] private Wine _wine;

    [RelayCommand]
    private async Task SetImageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            var result = await _filePicker.PickAsync();
            if (result != null)
            {
                if (!result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) &&
                    !result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    await Shell.Current.DisplayAlert("Format rejected!",
                        $"Please select a jpg or png file only.", "OK");
                    return;
                }

                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("No connectivity!",
                        $"Please check internet and try again.", "OK");
                    return;
                }

                IsBusy = true;

                await using var stream = await result.OpenReadAsync();
                var streamPart = new StreamPart(stream, result.FileName);
                Wine.ImageUrl = await _uploadManager.UploadAsync(streamPart);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to set an image: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

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

            if (Wine.Id == Guid.Empty)
                Wine = await _cellarApi.CreateWineAsync(Wine);
            else
                await _cellarApi.UpdateWineAsync(Wine.Id, Wine);

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
