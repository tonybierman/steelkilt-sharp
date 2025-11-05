using System.ComponentModel;
using System.Runtime.CompilerServices;
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
/// Represents a ranged weapon with its characteristics and UI data-binding support
/// </summary>
public class RangedWeapon : INotifyPropertyChanged
{
    private string _name;
    private RangedWeaponType _type;
    private int _damage;
    private int _shortRange;
    private int _mediumRange;
    private int _longRange;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public RangedWeaponType Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public int Damage
    {
        get => _damage;
        set => SetProperty(ref _damage, value);
    }

    public int ShortRange
    {
        get => _shortRange;
        set => SetProperty(ref _shortRange, value);
    }

    public int MediumRange
    {
        get => _mediumRange;
        set => SetProperty(ref _mediumRange, value);
    }

    public int LongRange
    {
        get => _longRange;
        set => SetProperty(ref _longRange, value);
    }

    public RangedWeapon(string name, RangedWeaponType type, int damage, int shortRange, int mediumRange, int longRange)
    {
        _name = name;
        _type = type;
        _damage = damage;
        _shortRange = shortRange;
        _mediumRange = mediumRange;
        _longRange = longRange;
    }

    public static RangedWeapon ShortBow() => new("Short Bow", RangedWeaponType.Bow, 4, 30, 60, 150);
    public static RangedWeapon LongBow() => new("Long Bow", RangedWeaponType.Bow, 6, 50, 100, 250);
    public static RangedWeapon LightCrossbow() => new("Light Crossbow", RangedWeaponType.Crossbow, 5, 40, 80, 200);
    public static RangedWeapon HeavyCrossbow() => new("Heavy Crossbow", RangedWeaponType.Crossbow, 8, 50, 100, 250);

    /// <summary>
    /// Sets a property and raises PropertyChanged event if value changed
    /// </summary>
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
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
/// Result of a ranged attack with UI data-binding support
/// </summary>
public class RangedCombatResult : INotifyPropertyChanged
{
    private string _attacker;
    private string _defender;
    private int _attackRoll;
    private int _defenseRoll;
    private RangeCategory _range;
    private bool _hit;
    private int _damage;
    private WoundLevel? _woundLevel;
    private bool _defenderDied;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Attacker
    {
        get => _attacker;
        set => SetProperty(ref _attacker, value);
    }

    public string Defender
    {
        get => _defender;
        set => SetProperty(ref _defender, value);
    }

    public int AttackRoll
    {
        get => _attackRoll;
        set => SetProperty(ref _attackRoll, value);
    }

    public int DefenseRoll
    {
        get => _defenseRoll;
        set => SetProperty(ref _defenseRoll, value);
    }

    public RangeCategory Range
    {
        get => _range;
        set => SetProperty(ref _range, value);
    }

    public bool Hit
    {
        get => _hit;
        set => SetProperty(ref _hit, value);
    }

    public int Damage
    {
        get => _damage;
        set => SetProperty(ref _damage, value);
    }

    public WoundLevel? WoundLevel
    {
        get => _woundLevel;
        set => SetProperty(ref _woundLevel, value);
    }

    public bool DefenderDied
    {
        get => _defenderDied;
        set => SetProperty(ref _defenderDied, value);
    }

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
        _attacker = attacker;
        _defender = defender;
        _attackRoll = attackRoll;
        _defenseRoll = defenseRoll;
        _range = range;
        _hit = hit;
        _damage = damage;
        _woundLevel = woundLevel;
        _defenderDied = defenderDied;
    }

    /// <summary>
    /// Sets a property and raises PropertyChanged event if value changed
    /// </summary>
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
