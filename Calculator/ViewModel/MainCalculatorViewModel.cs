using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculator.ViewModel;

public partial class MainCalculatorViewModel : ObservableObject
{

    public MainCalculatorViewModel()
    {
    }

    [ObservableProperty] private string input;

    [RelayCommand]
    private async Task Calculate()
    {
    }
}