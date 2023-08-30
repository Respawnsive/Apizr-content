using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarCellar.App.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        [ObservableProperty]
        string _name;

        [ObservableProperty]
        string _password;

        [ObservableProperty]
        string _email;

        [RelayCommand]
        public async Task SaveAsync()
        {}
    }
}
