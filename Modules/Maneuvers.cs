namespace SteelkiltSharp.Modules;

/// <summary>
/// Special combat maneuvers that modify combat rolls
/// </summary>
public enum ManeuverType
{
    Normal,
    Charge,
    AllOutAttack,
    DefensivePosition
}

/// <summary>
/// Represents modifiers applied by a combat maneuver
/// </summary>
public class ManeuverModifiers
{
    public int AttackBonus { get; set; }
    public int DefenseBonus { get; set; }
    public int DamageBonus { get; set; }

    public ManeuverModifiers(int attackBonus, int defenseBonus, int damageBonus)
    {
        AttackBonus = attackBonus;
        DefenseBonus = defenseBonus;
        DamageBonus = damageBonus;
    }
}

/// <summary>
/// Handles special combat maneuvers
/// </summary>
public static class Maneuvers
{
    /// <summary>
    /// Gets the modifiers for a specific maneuver
    /// </summary>
    public static ManeuverModifiers GetModifiers(ManeuverType maneuver)
    {
        return maneuver switch
        {
            ManeuverType.Charge => new ManeuverModifiers(
                attackBonus: 2,
                defenseBonus: -2,
                damageBonus: 2),

            ManeuverType.AllOutAttack => new ManeuverModifiers(
                attackBonus: 4,
                defenseBonus: -4,
                damageBonus: 0),

            ManeuverType.DefensivePosition => new ManeuverModifiers(
                attackBonus: -2,
                defenseBonus: 4,
                damageBonus: 0),

            _ => new ManeuverModifiers(0, 0, 0)
        };
    }

    /// <summary>
    /// Returns a description of the maneuver
    /// </summary>
    public static string GetDescription(ManeuverType maneuver)
    {
        return maneuver switch
        {
            ManeuverType.Charge => "Charge: +2 attack, -2 defense, +2 damage",
            ManeuverType.AllOutAttack => "All-Out Attack: +4 attack, -4 defense",
            ManeuverType.DefensivePosition => "Defensive Position: -2 attack, +4 defense",
            _ => "Normal attack"
        };
    }
}
