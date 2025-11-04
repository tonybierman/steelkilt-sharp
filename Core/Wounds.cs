namespace SteelkiltSharp.Core;

/// <summary>
/// Represents wound severity levels
/// </summary>
public enum WoundLevel
{
    Light,
    Severe,
    Critical
}

/// <summary>
/// Tracks character wounds with automatic stacking mechanics:
/// - 4 Light wounds convert to 1 Severe wound
/// - 3 Severe wounds convert to 1 Critical wound
/// - 2+ Critical wounds result in death
/// </summary>
public class Wounds
{
    public int Light { get; private set; }
    public int Severe { get; private set; }
    public int Critical { get; private set; }

    public Wounds()
    {
        Light = 0;
        Severe = 0;
        Critical = 0;
    }

    /// <summary>
    /// Adds a wound of the specified level and handles automatic stacking
    /// </summary>
    public void AddWound(WoundLevel level)
    {
        switch (level)
        {
            case WoundLevel.Light:
                Light++;
                if (Light >= 4)
                {
                    Light -= 4;
                    Severe++;
                }
                break;

            case WoundLevel.Severe:
                Severe++;
                break;

            case WoundLevel.Critical:
                Critical++;
                break;
        }

        // Handle severe wound stacking
        if (Severe >= 3)
        {
            Severe -= 3;
            Critical++;
        }
    }

    /// <summary>
    /// Returns true if the character has reached death threshold (2+ critical wounds)
    /// </summary>
    public bool IsDead => Critical >= 2;

    /// <summary>
    /// Calculates movement penalty based on wound severity
    /// </summary>
    public int MovementPenalty
    {
        get
        {
            int penalty = 0;
            penalty += Light;
            penalty += Severe * 2;
            penalty += Critical * 3;
            return -penalty;
        }
    }

    /// <summary>
    /// Returns total wound penalty for skill rolls
    /// </summary>
    public int TotalPenalty
    {
        get
        {
            int penalty = 0;
            penalty += Light * 1;
            penalty += Severe * 2;
            penalty += Critical * 4;
            return -penalty;
        }
    }

    public override string ToString()
    {
        return $"Light: {Light}, Severe: {Severe}, Critical: {Critical}";
    }
}
