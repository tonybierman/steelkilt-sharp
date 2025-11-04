namespace SteelkiltSharp.Core;

/// <summary>
/// Represents weapon impact classes in the Draft RPG system
/// </summary>
public enum WeaponImpact
{
    Small = 1,
    Medium = 2,
    Large = 3,
    Huge = 4
}

/// <summary>
/// Represents a weapon with its combat characteristics
/// </summary>
public class Weapon
{
    public string Name { get; set; }
    public WeaponImpact Impact { get; set; }

    /// <summary>
    /// Base damage calculated as (impact × 2) + 1
    /// </summary>
    public int Damage => ((int)Impact * 2) + 1;

    public Weapon(string name, WeaponImpact impact)
    {
        Name = name;
        Impact = impact;
    }

    /// <summary>
    /// Creates a dagger (Small impact)
    /// </summary>
    public static Weapon Dagger() => new("Dagger", WeaponImpact.Small);

    /// <summary>
    /// Creates a long sword (Medium impact)
    /// </summary>
    public static Weapon LongSword() => new("Long Sword", WeaponImpact.Medium);

    /// <summary>
    /// Creates a two-handed sword (Large impact)
    /// </summary>
    public static Weapon TwoHandedSword() => new("Two-Handed Sword", WeaponImpact.Large);

    /// <summary>
    /// Creates a great axe (Huge impact)
    /// </summary>
    public static Weapon GreatAxe() => new("Great Axe", WeaponImpact.Huge);
}
