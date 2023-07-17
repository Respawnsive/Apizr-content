using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Apizr.Sample.Content.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CountText))]
        [NotifyCanExecuteChangedFor(nameof(IncreaseCounterCommand))]
        private int _count;

        public string CountText
        {
            get
            {
                var text = "Click me";
                if (Count is > 0 and < 11)
                {
                    text = $"Clicked {Count} " + (Count == 1 ? "time" : "times");
                }
                SemanticScreenReader.Announce(text);
                return text;
            }
        }

        [RelayCommand(CanExecute = nameof(CanIncreaseCounter))]
        private void IncreaseCounter()
        {
            Count++;
        }

        private bool CanIncreaseCounter() => Count is >= 0 and < 10;
    }
}
