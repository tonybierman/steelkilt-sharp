using SteelkiltSharp.Core;

namespace SteelkiltSharp.Modules;

/// <summary>
/// Nine branches of magic in the Draft RPG system
/// </summary>
public enum MagicBranch
{
    Divination,
    Alchemy,
    Elementalism,
    Enchantment,
    Illusion,
    Necromancy,
    Summoning,
    Transmutation,
    Abjuration
}

/// <summary>
/// Magic spell power levels
/// </summary>
public enum SpellLevel
{
    Minor = 1,
    Lesser = 2,
    Moderate = 3,
    Greater = 4,
    Master = 5
}

/// <summary>
/// Represents a magical spell
/// </summary>
public class Spell
{
    public string Name { get; set; }
    public MagicBranch Branch { get; set; }
    public SpellLevel Level { get; set; }
    public int CastingTime { get; set; } // in rounds
    public int ExhaustionCost { get; set; }
    public string Description { get; set; }

    public Spell(string name, MagicBranch branch, SpellLevel level, int castingTime, int exhaustionCost, string description)
    {
        Name = name;
        Branch = branch;
        Level = level;
        CastingTime = castingTime;
        ExhaustionCost = exhaustionCost;
        Description = description;
    }

    /// <summary>
    /// Calculates the difficulty class for casting this spell
    /// </summary>
    public int DifficultyClass => 10 + ((int)Level * 2);
}

/// <summary>
/// Represents a character's magical abilities
/// </summary>
public class MagicUser
{
    public Dictionary<MagicBranch, int> BranchSkills { get; set; }
    public List<Spell> PreparedSpells { get; set; }
    public int MagicalExhaustion { get; set; }

    public MagicUser()
    {
        BranchSkills = new Dictionary<MagicBranch, int>();
        PreparedSpells = new List<Spell>();
        MagicalExhaustion = 0;

        // Initialize all branches to 0
        foreach (MagicBranch branch in Enum.GetValues<MagicBranch>())
        {
            BranchSkills[branch] = 0;
        }
    }

    /// <summary>
    /// Sets skill level for a specific magic branch
    /// </summary>
    public void SetBranchSkill(MagicBranch branch, int skill)
    {
        BranchSkills[branch] = Math.Max(0, Math.Min(10, skill));
    }

    /// <summary>
    /// Gets skill level for a specific magic branch
    /// </summary>
    public int GetBranchSkill(MagicBranch branch)
    {
        return BranchSkills.GetValueOrDefault(branch, 0);
    }

    /// <summary>
    /// Prepares a spell for casting
    /// </summary>
    public void PrepareSpell(Spell spell)
    {
        if (!PreparedSpells.Contains(spell))
        {
            PreparedSpells.Add(spell);
        }
    }
}

/// <summary>
/// Result of a spell casting attempt
/// </summary>
public class SpellCastResult
{
    public string CasterName { get; set; }
    public Spell Spell { get; set; }
    public int CastingRoll { get; set; }
    public int DifficultyClass { get; set; }
    public bool Success { get; set; }
    public int ExhaustionGained { get; set; }
    public string Message { get; set; }

    public SpellCastResult(string casterName, Spell spell, int castingRoll, int difficultyClass, bool success, int exhaustionGained, string message)
    {
        CasterName = casterName;
        Spell = spell;
        CastingRoll = castingRoll;
        DifficultyClass = difficultyClass;
        Success = success;
        ExhaustionGained = exhaustionGained;
        Message = message;
    }

    public override string ToString()
    {
        string result = Success ? "SUCCESS" : "FAILED";
        return $"{CasterName} casts {Spell.Name}: {result} (Roll:{CastingRoll} vs DC:{DifficultyClass}) - {Message}";
    }
}

/// <summary>
/// Handles magic casting mechanics
/// </summary>
public static class MagicSystem
{
    /// <summary>
    /// Attempts to cast a spell
    /// </summary>
    public static SpellCastResult CastSpell(Character caster, Spell spell)
    {
        if (caster.Magic == null)
        {
            throw new InvalidOperationException("Character is not a magic user");
        }

        if (!caster.Magic.PreparedSpells.Contains(spell))
        {
            return new SpellCastResult(
                caster.Name,
                spell,
                0,
                spell.DifficultyClass,
                success: false,
                exhaustionGained: 0,
                message: "Spell not prepared");
        }

        // Get caster's skill in this branch
        int branchSkill = caster.Magic.GetBranchSkill(spell.Branch);

        // Calculate casting roll
        int castingRoll = branchSkill +
                         Dice.D10() +
                         caster.Wounds.TotalPenalty +
                         ExhaustionSystem.GetPenalty(caster.Exhaustion) +
                         ExhaustionSystem.GetPenalty(caster.Magic.MagicalExhaustion);

        bool success = castingRoll >= spell.DifficultyClass;

        // Apply exhaustion cost
        int exhaustionCost = success ? spell.ExhaustionCost : (spell.ExhaustionCost / 2);
        caster.Magic.MagicalExhaustion += exhaustionCost;
        caster.Exhaustion += exhaustionCost;

        string message = success
            ? $"Successfully cast {spell.Name}!"
            : $"Failed to cast {spell.Name}. Gained {exhaustionCost} exhaustion anyway.";

        return new SpellCastResult(
            caster.Name,
            spell,
            castingRoll,
            spell.DifficultyClass,
            success,
            exhaustionCost,
            message);
    }

    /// <summary>
    /// Creates a combat spell with damage
    /// </summary>
    public static Spell CreateCombatSpell(string name, MagicBranch branch, SpellLevel level)
    {
        int damage = (int)level * 3;
        int castingTime = (int)level;
        int exhaustionCost = (int)level * 2;

        return new Spell(
            name,
            branch,
            level,
            castingTime,
            exhaustionCost,
            $"Deals {damage} damage to target");
    }

    /// <summary>
    /// Common spell library
    /// </summary>
    public static class Spells
    {
        public static Spell Fireball() => CreateCombatSpell("Fireball", MagicBranch.Elementalism, SpellLevel.Moderate);
        public static Spell LightningBolt() => CreateCombatSpell("Lightning Bolt", MagicBranch.Elementalism, SpellLevel.Greater);
        public static Spell MagicMissile() => CreateCombatSpell("Magic Missile", MagicBranch.Abjuration, SpellLevel.Minor);
        public static Spell IceSpear() => CreateCombatSpell("Ice Spear", MagicBranch.Elementalism, SpellLevel.Lesser);

        public static Spell Heal() => new Spell(
            "Heal",
            MagicBranch.Enchantment,
            SpellLevel.Moderate,
            castingTime: 2,
            exhaustionCost: 4,
            "Heals 1d10 hit points worth of wounds");

        public static Spell DetectMagic() => new Spell(
            "Detect Magic",
            MagicBranch.Divination,
            SpellLevel.Minor,
            castingTime: 1,
            exhaustionCost: 1,
            "Reveals magical auras within 30 feet");

        public static Spell Shield() => new Spell(
            "Shield",
            MagicBranch.Abjuration,
            SpellLevel.Lesser,
            castingTime: 1,
            exhaustionCost: 2,
            "Grants +2 to defense for 5 rounds");
    }
}
