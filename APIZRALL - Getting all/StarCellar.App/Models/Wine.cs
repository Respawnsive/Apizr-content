namespace StarCellar.App.Models;

public partial class Wine : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private string _imageUrl;
    [ObservableProperty] private int _stock;
    [ObservableProperty] private int _score;
}