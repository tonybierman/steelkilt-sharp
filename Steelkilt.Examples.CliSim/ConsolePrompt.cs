using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steelkilt.Examples.CliSim
{
    public static class ConsolePrompt
    {
        public static T GetUserChoice<T>(
            string promptPrefix,
            Dictionary<char, T> options,
            T defaultValue) where T : notnull
        {
            var promptSuffix = GeneratePromptSuffix(options, defaultValue);
            var fullPrompt = $"{promptPrefix} {promptSuffix}: ";

            while (true)
            {
                Console.Write(fullPrompt);
                var input = Console.ReadLine()?.ToUpper() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    return defaultValue;

                if (options.TryGetValue(input[0], out var value))
                    return value;

                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        private static string GeneratePromptSuffix<T>(
            Dictionary<char, T> options,
            T defaultValue) where T : notnull
        {
            var parts = options.Select(kvp =>
            {
                var key = kvp.Key;
                var value = kvp.Value.ToString() ?? "";
                var isDefault = kvp.Value.Equals(defaultValue);

                return $"[{key}]{value}";

                //if (isDefault)
                //    return $"[{key}]{value}";
                //else
                //    return $"{key}{value}";
            });

            var defaultValueStr = defaultValue.ToString() ?? "";
            return "Select " + string.Join(", ", parts) + $" (default: {defaultValueStr})";
        }
    }
}
