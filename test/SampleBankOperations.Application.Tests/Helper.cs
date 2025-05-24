using System;
using System.IO;

namespace SampleBankOperations.Application.Tests.Application.Utils
{
    public static class Helper
    {
        public static void WithSimulatedInput(Action action, params string[] inputs)
        {
            var originalIn = Console.In;
            try
            {
                var combinedInput = string.Join(Environment.NewLine, inputs);
                using var reader = new StringReader(combinedInput);
                Console.SetIn(reader);
                action();
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        public static string WithSimulatedOutput(Action action)
        {
            var originalOut = Console.Out;
            try
            {
                using var writer = new StringWriter();
                Console.SetOut(writer);
                action();
                return NormalizeLineEndings(writer.ToString());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        private static string NormalizeLineEndings(string input)
        {
            return input.Replace("\r", "").Replace("\n", "");
        }
    }
}
