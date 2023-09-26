using StarCellar.App.Services.Navigation;

namespace StarCellar.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty] private string _title;

    public BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public bool IsNotBusy => !IsBusy;
}
