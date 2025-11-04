using SteelkiltSharp.Core;

namespace SteelkiltSharp.Modules;

/// <summary>
/// Types of ranged weapons
/// </summary>
public enum RangedWeaponType
{
    Bow,
    Crossbow,
    Firearm
}

/// <summary>
/// Represents a ranged weapon with its characteristics
/// </summary>
public class RangedWeapon
{
    public string Name { get; set; }
    public RangedWeaponType Type { get; set; }
    public int Damage { get; set; }
    public int ShortRange { get; set; }
    public int MediumRange { get; set; }
    public int LongRange { get; set; }

    public RangedWeapon(string name, RangedWeaponType type, int damage, int shortRange, int mediumRange, int longRange)
    {
        Name = name;
        Type = type;
        Damage = damage;
        ShortRange = shortRange;
        MediumRange = mediumRange;
        LongRange = longRange;
    }

    public static RangedWeapon ShortBow() => new("Short Bow", RangedWeaponType.Bow, 4, 30, 60, 150);
    public static RangedWeapon LongBow() => new("Long Bow", RangedWeaponType.Bow, 6, 50, 100, 250);
    public static RangedWeapon LightCrossbow() => new("Light Crossbow", RangedWeaponType.Crossbow, 5, 40, 80, 200);
    public static RangedWeapon HeavyCrossbow() => new("Heavy Crossbow", RangedWeaponType.Crossbow, 8, 50, 100, 250);
}

/// <summary>
/// Distance categories for ranged combat
/// </summary>
public enum RangeCategory
{
    PointBlank,
    Short,
    Medium,
    Long,
    OutOfRange
}

/// <summary>
/// Result of a ranged attack
/// </summary>
public class RangedCombatResult
{
    public string Attacker { get; set; }
    public string Defender { get; set; }
    public int AttackRoll { get; set; }
    public int DefenseRoll { get; set; }
    public RangeCategory Range { get; set; }
    public bool Hit { get; set; }
    public int Damage { get; set; }
    public WoundLevel? WoundLevel { get; set; }
    public bool DefenderDied { get; set; }

    public RangedCombatResult(
        string attacker,
        string defender,
        int attackRoll,
        int defenseRoll,
        RangeCategory range,
        bool hit,
        int damage,
        WoundLevel? woundLevel,
        bool defenderDied)
    {
        Attacker = attacker;
        Defender = defender;
        AttackRoll = attackRoll;
        DefenseRoll = defenseRoll;
        Range = range;
        Hit = hit;
        Damage = damage;
        WoundLevel = woundLevel;
        DefenderDied = defenderDied;
    }

    public override string ToString()
    {
        if (!Hit)
        {
            return $"{Attacker} shoots at {Defender} ({Range} range): MISS (Attack:{AttackRoll} vs Defense:{DefenseRoll})";
        }

        string woundStr = WoundLevel.HasValue ? $" - {WoundLevel.Value} wound" : "";
        string deathStr = DefenderDied ? " - DEFENDER DIED!" : "";

        return $"{Attacker} shoots at {Defender} ({Range} range): HIT for {Damage} damage" +
               $" (Attack:{AttackRoll} vs Defense:{DefenseRoll}){woundStr}{deathStr}";
    }
}

/// <summary>
/// Handles ranged combat mechanics
/// </summary>
public static class RangedCombat
{
    /// <summary>
    /// Determines range category based on distance
    /// </summary>
    public static RangeCategory DetermineRange(int distance, RangedWeapon weapon)
    {
        if (distance <= 5) return RangeCategory.PointBlank;
        if (distance <= weapon.ShortRange) return RangeCategory.Short;
        if (distance <= weapon.MediumRange) return RangeCategory.Medium;
        if (distance <= weapon.LongRange) return RangeCategory.Long;
        return RangeCategory.OutOfRange;
    }

    /// <summary>
    /// Gets range modifier for attack roll
    /// </summary>
    public static int GetRangeModifier(RangeCategory range)
    {
        return range switch
        {
            RangeCategory.PointBlank => 2,
            RangeCategory.Short => 0,
            RangeCategory.Medium => -2,
            RangeCategory.Long => -4,
            RangeCategory.OutOfRange => -10,
            _ => 0
        };
    }

    /// <summary>
    /// Executes a ranged combat round
    /// </summary>
    public static RangedCombatResult RangedAttack(
        Character attacker,
        Character defender,
        int distance,
        bool aiming = false,
        int coverPenalty = 0)
    {
        if (attacker.RangedWeapon == null || !attacker.RangedSkill.HasValue)
        {
            throw new InvalidOperationException("Attacker does not have a ranged weapon or ranged skill");
        }

        RangeCategory range = DetermineRange(distance, attacker.RangedWeapon);
        int rangeModifier = GetRangeModifier(range);
        int aimingBonus = aiming ? 2 : 0;

        // Attacker's roll
        int attackRoll = attacker.RangedSkill.Value + Dice.D10() +
                        rangeModifier +
                        aimingBonus +
                        attacker.Wounds.TotalPenalty +
                        ExhaustionSystem.GetPenalty(attacker.Exhaustion) +
                        coverPenalty;

        // Defender dodges
        int defenseRoll = defender.DodgeRoll();

        bool hit = attackRoll > defenseRoll && range != RangeCategory.OutOfRange;

        if (!hit)
        {
            return new RangedCombatResult(
                attacker.Name,
                defender.Name,
                attackRoll,
                defenseRoll,
                range,
                hit: false,
                damage: 0,
                woundLevel: null,
                defenderDied: false);
        }

        // Calculate damage
        int rawDamage = (attackRoll - defenseRoll) +
                        attacker.RangedWeapon.Damage -
                        defender.Armor.Protection;

        int damage = Math.Max(0, rawDamage);

        // Determine wound level
        WoundLevel? woundLevel = null;
        if (damage > 0)
        {
            woundLevel = DetermineWoundLevel(damage, defender.Attributes.Constitution);
            defender.Wounds.AddWound(woundLevel.Value);
        }

        bool defenderDied = defender.IsDead;

        return new RangedCombatResult(
            attacker.Name,
            defender.Name,
            attackRoll,
            defenseRoll,
            range,
            hit: true,
            damage,
            woundLevel,
            defenderDied);
    }

    /// <summary>
    /// Determines wound level based on damage relative to defender's constitution
    /// </summary>
    private static WoundLevel DetermineWoundLevel(int damage, int constitution)
    {
        if (damage >= constitution * 2)
        {
            return WoundLevel.Critical;
        }
        else if (damage >= constitution)
        {
            return WoundLevel.Severe;
        }
        else
        {
            return WoundLevel.Light;
        }
    }
}
