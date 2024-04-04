using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pfz.Math;
using Pfz.Math.Parser;
using System.Linq.Expressions;

namespace Calculator.ViewModel;

public partial class MainCalculatorViewModel : ObservableObject
{

    public MainCalculatorViewModel()
    {
    }

    [ObservableProperty] private string input;
    [ObservableProperty] private string output;

    [RelayCommand]
    private async Task Calculate()
    {

        try
        {
            if (string.IsNullOrEmpty(Input))
                return;

            var evaluator = Evaluator.Create();
            var result = evaluator.Evaluate(Input);
            Output = result.ToString();

        }
        catch (Exception ex)
        {
            Output = ex.Message;
        }
    }

    
}


public class Evaluator
{
    public MathVariable PriorVariable { get; set; }
    public MathParser Parser { get; set; }

    public BigDecimal Evaluate(string input)
    {
        Expression<Func<BigDecimal>> func = Parser.Parse(input);
        var compiled = func.Compile();
        BigDecimal result = compiled.Invoke();

        PriorVariable.Value = result;
        return PriorVariable.Value;
    }

    public static Evaluator Create()
    {
        var parser = new MathParser();
        parser.RegisterFunction("Divide", _CompatibleDivide);
        parser.RegisterFunction("Power", _Power2);
        parser.RegisterFunction("Power2", _Power2);
        parser.RegisterFunction("Power3", _Power3);
        parser.RegisterFunction("Sqrt", _Sqrt);
        var priorVariable = parser.DeclareVariable("prior");
        priorVariable.Value = new BigDecimal(0);

        var evaluator = new Evaluator();
        evaluator.Parser = parser;
        evaluator.PriorVariable = priorVariable;
        return evaluator;
    }

    static BigDecimal _CompatibleDivide(BigDecimal dividend, BigDecimal divisor, BigDecimal digitCount)
    {
        var realDigitCount = digitCount.ToBigInteger();
        if (realDigitCount < 0)
            realDigitCount = 0;
        else if (realDigitCount > 1000)
            realDigitCount = 1000;

        var result = BigDecimal.Divide(dividend, divisor, realDigitCount);
        return result;
    }

    static BigDecimal _Power2(BigDecimal value, BigDecimal exponent)
    {
        return value.Power(exponent.ToBigInteger());
    }

    static BigDecimal _Power3(BigDecimal value, BigDecimal exponent, BigDecimal digitCount)
    {
        var realDigitCount = digitCount.ToBigInteger();
        if (realDigitCount < 0)
            realDigitCount = 0;
        else if (realDigitCount > 1000)
            realDigitCount = 1000;

        return value.Power(exponent.ToBigInteger(), realDigitCount);
    }

    static BigDecimal _Sqrt(BigDecimal value)
    {
        if(decimal.TryParse(value.ToString(), out var dec))
        {
            return BigDecimal.Parse(Sqrt(dec).ToString());
        }

        throw new CalculatorException("слишком большой корень");
    }
    public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
    {
        if (x < 0) throw new CalculatorException("невозможно вычислить корень из отрицательного числа.");
        decimal current = (decimal)Math.Sqrt((double)x), previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) / 2;
        }
        while (Math.Abs(previous - current) > epsilon);
        return current;
    }

}