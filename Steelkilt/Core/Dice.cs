namespace SteelkiltSharp.Core;

/// <summary>
/// Utility class for dice rolling
/// </summary>
public static class Dice
{
    private static readonly Random _random = new();

    /// <summary>
    /// Rolls a 10-sided die (returns 1-10)
    /// </summary>
    public static int D10() => _random.Next(1, 11);

    /// <summary>
    /// Rolls a specified number of 10-sided dice
    /// </summary>
    public static int RollMultiple(int count)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
        {
            total += D10();
        }
        return total;
    }
}
