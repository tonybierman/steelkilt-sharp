using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Core;

/// <summary>
/// Represents a combatant in the Draft RPG system
/// </summary>
public class Character
{
    public string Name { get; set; }
    public Attributes Attributes { get; set; }
    public int WeaponSkill { get; set; }
    public int DodgeSkill { get; set; }
    public Weapon Weapon { get; set; }
    public Armor Armor { get; set; }
    public Wounds Wounds { get; set; }

    // Optional advanced features
    public MagicUser? Magic { get; set; }
    public RangedWeapon? RangedWeapon { get; set; }
    public int? RangedSkill { get; set; }
    public int Exhaustion { get; set; }

    public Character(
        string name,
        Attributes attributes,
        int weaponSkill,
        int dodgeSkill,
        Weapon weapon,
        Armor armor)
    {
        Name = name;
        Attributes = attributes;
        WeaponSkill = ClampSkill(weaponSkill);
        DodgeSkill = ClampSkill(dodgeSkill);
        Weapon = weapon;
        Armor = armor;
        Wounds = new Wounds();
        Exhaustion = 0;
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

    public override string ToString()
    {
        return $"{Name} (STR:{Attributes.Strength} DEX:{Attributes.Dexterity} CON:{Attributes.Constitution}) " +
               $"WeaponSkill:{WeaponSkill} DodgeSkill:{DodgeSkill} " +
               $"Weapon:{Weapon.Name} Armor:{Armor.Name}";
    }
}
