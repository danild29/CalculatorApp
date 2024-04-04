using System.Runtime.Serialization;

namespace Calculator.ViewModel;
[Serializable]
internal class CalculatorException : Exception
{
    public CalculatorException()
    {
    }

    public CalculatorException(string? message) : base(message)
    {
    }

    public CalculatorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CalculatorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}