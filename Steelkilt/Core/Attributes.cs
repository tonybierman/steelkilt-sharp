namespace SteelkiltSharp.Core;

/// <summary>
/// Represents the nine core attributes of a character in the Draft RPG system.
/// Attributes are organized into Physical, Mental, and Interactive categories.
/// All values are clamped between 1 and 10.
/// </summary>
public class Attributes
{
    private int _strength;
    private int _dexterity;
    private int _constitution;
    private int _reason;
    private int _intuition;
    private int _willpower;
    private int _charisma;
    private int _perception;
    private int _empathy;

    /// <summary>Physical: Strength attribute (1-10)</summary>
    public int Strength
    {
        get => _strength;
        set => _strength = Clamp(value);
    }

    /// <summary>Physical: Dexterity attribute (1-10)</summary>
    public int Dexterity
    {
        get => _dexterity;
        set => _dexterity = Clamp(value);
    }

    /// <summary>Physical: Constitution attribute (1-10)</summary>
    public int Constitution
    {
        get => _constitution;
        set => _constitution = Clamp(value);
    }

    /// <summary>Mental: Reason attribute (1-10)</summary>
    public int Reason
    {
        get => _reason;
        set => _reason = Clamp(value);
    }

    /// <summary>Mental: Intuition attribute (1-10)</summary>
    public int Intuition
    {
        get => _intuition;
        set => _intuition = Clamp(value);
    }

    /// <summary>Mental: Willpower attribute (1-10)</summary>
    public int Willpower
    {
        get => _willpower;
        set => _willpower = Clamp(value);
    }

    /// <summary>Interactive: Charisma attribute (1-10)</summary>
    public int Charisma
    {
        get => _charisma;
        set => _charisma = Clamp(value);
    }

    /// <summary>Interactive: Perception attribute (1-10)</summary>
    public int Perception
    {
        get => _perception;
        set => _perception = Clamp(value);
    }

    /// <summary>Interactive: Empathy attribute (1-10)</summary>
    public int Empathy
    {
        get => _empathy;
        set => _empathy = Clamp(value);
    }

    /// <summary>
    /// Calculates stamina as (Strength + Constitution) / 2
    /// </summary>
    public int Stamina => (Strength + Constitution) / 2;

    public Attributes(
        int strength,
        int dexterity,
        int constitution,
        int reason,
        int intuition,
        int willpower,
        int charisma,
        int perception,
        int empathy)
    {
        Strength = strength;
        Dexterity = dexterity;
        Constitution = constitution;
        Reason = reason;
        Intuition = intuition;
        Willpower = willpower;
        Charisma = charisma;
        Perception = perception;
        Empathy = empathy;
    }

    /// <summary>
    /// Clamps a value between 1 and 10
    /// </summary>
    private static int Clamp(int value)
    {
        return Math.Max(1, Math.Min(10, value));
    }
}
