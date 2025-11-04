namespace SteelkiltSharp.Core;

/// <summary>
/// Handles combat resolution for the Draft RPG system
/// </summary>
public static class Combat
{
    /// <summary>
    /// Executes a single combat round between attacker and defender
    /// </summary>
    public static CombatResult CombatRound(
        Character attacker,
        Character defender,
        DefenseAction defenseAction = DefenseAction.Parry)
    {
        // Roll attack and defense
        int attackRoll = attacker.AttackRoll();
        int defenseRoll = defenseAction == DefenseAction.Parry
            ? defender.ParryRoll()
            : defender.DodgeRoll();

        // Check if attack hits
        bool hit = attackRoll > defenseRoll;

        if (!hit)
        {
            return new CombatResult(
                attacker.Name,
                defender.Name,
                attackRoll,
                defenseRoll,
                hit: false,
                damage: 0,
                woundLevel: null,
                defenderDied: false);
        }

        // Calculate damage
        int rawDamage = (attackRoll - defenseRoll) +
                        attacker.StrengthBonus +
                        attacker.Weapon.Damage -
                        defender.Armor.Protection;

        int damage = Math.Max(0, rawDamage);

        // Determine wound level based on damage vs constitution
        WoundLevel? woundLevel = null;
        if (damage > 0)
        {
            woundLevel = DetermineWoundLevel(damage, defender.Attributes.Constitution);
            defender.Wounds.AddWound(woundLevel.Value);
        }

        bool defenderDied = defender.IsDead;

        return new CombatResult(
            attacker.Name,
            defender.Name,
            attackRoll,
            defenseRoll,
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
