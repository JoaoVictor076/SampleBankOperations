using SampleBankOperations.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SampleBankOperations.Infrastructure.Logging;

[ExcludeFromCodeCoverage]
public class Logger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
    }
}
