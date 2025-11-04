namespace SteelkiltSharp.Core;

/// <summary>
/// Represents armor types with protection values
/// </summary>
public enum ArmorType
{
    HeavyCloth = 1,
    Leather = 2,
    Chain = 3,
    Plate = 4,
    FullPlate = 5
}

/// <summary>
/// Represents armor with protection and movement penalty
/// </summary>
public class Armor
{
    public string Name { get; set; }
    public ArmorType ArmorType { get; set; }
    public int Protection { get; set; }
    public int MovementPenalty { get; set; }

    public Armor(string name, ArmorType armorType, int protection, int movementPenalty)
    {
        Name = name;
        ArmorType = armorType;
        Protection = protection;
        MovementPenalty = movementPenalty;
    }

    /// <summary>
    /// Creates no armor (0 protection, 0 penalty)
    /// </summary>
    public static Armor None() => new("None", ArmorType.HeavyCloth, 0, 0);

    /// <summary>
    /// Creates heavy cloth armor
    /// </summary>
    public static Armor HeavyCloth() => new("Heavy Cloth", ArmorType.HeavyCloth, 1, 0);

    /// <summary>
    /// Creates leather armor
    /// </summary>
    public static Armor Leather() => new("Leather", ArmorType.Leather, 2, 0);

    /// <summary>
    /// Creates chain mail armor
    /// </summary>
    public static Armor Chain() => new("Chain Mail", ArmorType.Chain, 3, -1);

    /// <summary>
    /// Creates plate armor
    /// </summary>
    public static Armor Plate() => new("Plate Armor", ArmorType.Plate, 4, -2);

    /// <summary>
    /// Creates full plate armor
    /// </summary>
    public static Armor FullPlate() => new("Full Plate", ArmorType.FullPlate, 5, -3);
}
