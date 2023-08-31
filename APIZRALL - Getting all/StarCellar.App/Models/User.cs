namespace StarCellar.App.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty] private string _id;
        [ObservableProperty] private string _userName;
        [ObservableProperty] private string _fullName;
        [ObservableProperty] private string _email;
        [ObservableProperty] private int _age;
        [ObservableProperty] private string _role;
        [ObservableProperty] private string _address;
    }
}
