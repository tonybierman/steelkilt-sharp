using System.ComponentModel;
using System.Runtime.CompilerServices;
using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Core;

/// <summary>
/// Represents a combatant in the Draft RPG system with UI data-binding support.
/// Automatically propagates child object property changes to enable proper UI re-rendering.
/// </summary>
public class Character : INotifyPropertyChanged
{
    private string _name;
    private Attributes _attributes;
    private int _weaponSkill;
    private int _dodgeSkill;
    private Weapon _weapon;
    private Armor _armor;
    private Wounds _wounds;
    private MagicUser? _magic;
    private RangedWeapon? _rangedWeapon;
    private int? _rangedSkill;
    private int _exhaustion;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public Attributes Attributes
    {
        get => _attributes;
        set => SetChildProperty(ref _attributes, value);
    }

    public int WeaponSkill
    {
        get => _weaponSkill;
        set => SetProperty(ref _weaponSkill, ClampSkill(value));
    }

    public int DodgeSkill
    {
        get => _dodgeSkill;
        set => SetProperty(ref _dodgeSkill, ClampSkill(value));
    }

    public Weapon Weapon
    {
        get => _weapon;
        set => SetChildProperty(ref _weapon, value);
    }

    public Armor Armor
    {
        get => _armor;
        set => SetChildProperty(ref _armor, value);
    }

    public Wounds Wounds
    {
        get => _wounds;
        set => SetChildProperty(ref _wounds, value);
    }

    // Optional advanced features
    public MagicUser? Magic
    {
        get => _magic;
        set => SetChildProperty(ref _magic, value);
    }

    public RangedWeapon? RangedWeapon
    {
        get => _rangedWeapon;
        set => SetChildProperty(ref _rangedWeapon, value);
    }

    public int? RangedSkill
    {
        get => _rangedSkill;
        set => SetProperty(ref _rangedSkill, value);
    }

    public int Exhaustion
    {
        get => _exhaustion;
        set => SetProperty(ref _exhaustion, value);
    }

    public Character(
        string name,
        Attributes attributes,
        int weaponSkill,
        int dodgeSkill,
        Weapon weapon,
        Armor armor)
    {
        _name = name;
        _weaponSkill = ClampSkill(weaponSkill);
        _dodgeSkill = ClampSkill(dodgeSkill);
        _exhaustion = 0;

        // Set child objects using the property setters to enable subscription
        Attributes = attributes;
        Weapon = weapon;
        Armor = armor;
        Wounds = new Wounds();
    }

    /// <summary>
    /// Calculates strength bonus for damage
    /// STR ≤ 2: -1
    /// STR ≥ 7: +1
    /// STR ≥ 9: +2
    /// </summary>
    public int StrengthBonus
    {
        get
        {
            if (Attributes.Strength <= 2) return -1;
            if (Attributes.Strength >= 9) return 2;
            if (Attributes.Strength >= 7) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Performs an attack roll: weapon skill + d10 + modifiers
    /// </summary>
    public int AttackRoll()
    {
        int roll = WeaponSkill + Dice.D10();
        roll += Wounds.TotalPenalty;
        roll += Armor.MovementPenalty;
        roll += ExhaustionSystem.GetPenalty(Exhaustion);
        return roll;
    }

    /// <summary>
    /// Performs a parry roll: weapon skill + d10 + modifiers
    /// </summary>
    public int ParryRoll()
    {
        int roll = WeaponSkill + Dice.D10();
        roll += Wounds.TotalPenalty;
        roll += Armor.MovementPenalty;
        roll += ExhaustionSystem.GetPenalty(Exhaustion);
        return roll;
    }

    /// <summary>
    /// Performs a dodge roll: dodge skill + d10 + modifiers
    /// </summary>
    public int DodgeRoll()
    {
        int roll = DodgeSkill + Dice.D10();
        roll += Wounds.TotalPenalty;
        roll += Armor.MovementPenalty;
        roll += ExhaustionSystem.GetPenalty(Exhaustion);
        return roll;
    }

    /// <summary>
    /// Returns true if character is dead
    /// </summary>
    public bool IsDead => Wounds.IsDead;

    /// <summary>
    /// Clamps skill values between 0 and 10
    /// </summary>
    private static int ClampSkill(int skill)
    {
        return Math.Max(0, Math.Min(10, skill));
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
    /// Sets a child object property and subscribes to its PropertyChanged events.
    /// Child property changes automatically propagate up to the parent.
    /// </summary>
    private void SetChildProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        where T : INotifyPropertyChanged
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            // Unsubscribe from old child object
            if (field != null)
            {
                field.PropertyChanged -= OnChildPropertyChanged;
            }

            field = value;

            // Subscribe to new child object
            if (field != null)
            {
                field.PropertyChanged += OnChildPropertyChanged;
            }

            OnPropertyChanged(propertyName);
        }
    }

    /// <summary>
    /// Handles PropertyChanged events from child objects and propagates them up.
    /// This ensures UI re-renders when child properties change.
    /// </summary>
    private void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Determine which child object changed and propagate the change
        if (sender == _attributes)
        {
            OnPropertyChanged(nameof(Attributes));
        }
        else if (sender == _weapon)
        {
            OnPropertyChanged(nameof(Weapon));
        }
        else if (sender == _armor)
        {
            OnPropertyChanged(nameof(Armor));
        }
        else if (sender == _wounds)
        {
            OnPropertyChanged(nameof(Wounds));
        }
        else if (sender == _magic)
        {
            OnPropertyChanged(nameof(Magic));
        }
        else if (sender == _rangedWeapon)
        {
            OnPropertyChanged(nameof(RangedWeapon));
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
        return $"{Name} (STR:{Attributes.Strength} DEX:{Attributes.Dexterity} CON:{Attributes.Constitution}) " +
               $"WeaponSkill:{WeaponSkill} DodgeSkill:{DodgeSkill} " +
               $"Weapon:{Weapon.Name} Armor:{Armor.Name}";
    }
}