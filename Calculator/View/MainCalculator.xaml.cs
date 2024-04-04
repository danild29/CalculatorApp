using Calculator.Helpers;
using Calculator.ViewModel;

namespace Calculator.View;

public partial class MainCalculator : ContentPage
{
	public MainCalculator()
	{
        BindingContext = ServiceHelper.GetService<MainCalculatorViewModel>();
		InitializeComponent();
	}
}
