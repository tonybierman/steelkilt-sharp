namespace SteelkiltSharp.Modules;

/// <summary>
/// Manages exhaustion tracking and penalties in combat
/// </summary>
public static class ExhaustionSystem
{
    /// <summary>
    /// Returns the penalty for a given exhaustion level
    /// </summary>
    public static int GetPenalty(int exhaustion)
    {
        if (exhaustion <= 0) return 0;
        if (exhaustion <= 5) return -1;
        if (exhaustion <= 10) return -2;
        if (exhaustion <= 15) return -3;
        return -4;
    }

    /// <summary>
    /// Adds exhaustion from performing a combat action
    /// </summary>
    public static int AddCombatExhaustion(int currentExhaustion, int amount = 1)
    {
        return currentExhaustion + amount;
    }

    /// <summary>
    /// Recovers exhaustion during rest
    /// </summary>
    public static int RecoverExhaustion(int currentExhaustion, int amount = 1)
    {
        return Math.Max(0, currentExhaustion - amount);
    }

    /// <summary>
    /// Gets a description of the current exhaustion state
    /// </summary>
    public static string GetExhaustionDescription(int exhaustion)
    {
        if (exhaustion <= 0) return "Fresh";
        if (exhaustion <= 5) return "Slightly Tired (-1)";
        if (exhaustion <= 10) return "Tired (-2)";
        if (exhaustion <= 15) return "Exhausted (-3)";
        return "Completely Exhausted (-4)";
    }
}
