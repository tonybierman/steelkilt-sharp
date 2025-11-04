using SteelkiltSharp.Core;

namespace SteelkiltSharp.Modules;

/// <summary>
/// Body locations that can be targeted in combat
/// </summary>
public enum BodyLocation
{
    Head,
    Torso,
    RightArm,
    LeftArm,
    RightLeg,
    LeftLeg
}

/// <summary>
/// Result of a hit location roll
/// </summary>
public class HitLocationResult
{
    public BodyLocation Location { get; set; }
    public double DamageMultiplier { get; set; }
    public string Description { get; set; }

    public HitLocationResult(BodyLocation location, double damageMultiplier, string description)
    {
        Location = location;
        DamageMultiplier = damageMultiplier;
        Description = description;
    }
}

/// <summary>
/// Handles hit location mechanics with targeted damage multipliers
/// </summary>
public static class HitLocation
{
    /// <summary>
    /// Rolls for a random hit location
    /// </summary>
    public static HitLocationResult RollHitLocation()
    {
        int roll = Dice.D10();

        return roll switch
        {
            1 or 2 => new HitLocationResult(BodyLocation.Head, 1.5, "Head (1.5x damage)"),
            3 or 4 or 5 or 6 => new HitLocationResult(BodyLocation.Torso, 1.0, "Torso (normal damage)"),
            7 => new HitLocationResult(BodyLocation.RightArm, 0.75, "Right Arm (0.75x damage)"),
            8 => new HitLocationResult(BodyLocation.LeftArm, 0.75, "Left Arm (0.75x damage)"),
            9 => new HitLocationResult(BodyLocation.RightLeg, 0.75, "Right Leg (0.75x damage)"),
            10 => new HitLocationResult(BodyLocation.LeftLeg, 0.75, "Left Leg (0.75x damage)"),
            _ => new HitLocationResult(BodyLocation.Torso, 1.0, "Torso (normal damage)")
        };
    }

    /// <summary>
    /// Gets hit location with damage multiplier for a specific body part
    /// </summary>
    public static HitLocationResult GetLocation(BodyLocation location)
    {
        return location switch
        {
            BodyLocation.Head => new HitLocationResult(location, 1.5, "Head (1.5x damage)"),
            BodyLocation.Torso => new HitLocationResult(location, 1.0, "Torso (normal damage)"),
            BodyLocation.RightArm => new HitLocationResult(location, 0.75, "Right Arm (0.75x damage)"),
            BodyLocation.LeftArm => new HitLocationResult(location, 0.75, "Left Arm (0.75x damage)"),
            BodyLocation.RightLeg => new HitLocationResult(location, 0.75, "Right Leg (0.75x damage)"),
            BodyLocation.LeftLeg => new HitLocationResult(location, 0.75, "Left Leg (0.75x damage)"),
            _ => new HitLocationResult(location, 1.0, "Torso (normal damage)")
        };
    }

    /// <summary>
    /// Applies hit location damage multiplier to damage value
    /// </summary>
    public static int ApplyLocationDamage(int baseDamage, BodyLocation location)
    {
        var result = GetLocation(location);
        return (int)Math.Round(baseDamage * result.DamageMultiplier);
    }
}
