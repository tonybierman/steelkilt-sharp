namespace SteelkiltSharp.Modules;

/// <summary>
/// Skill difficulty levels in the Draft RPG system
/// </summary>
public enum SkillDifficulty
{
    Easy,
    Average,
    Hard,
    VeryHard
}

/// <summary>
/// Represents a learnable skill with progression mechanics
/// </summary>
public class Skill
{
    public string Name { get; set; }
    public SkillDifficulty Difficulty { get; set; }
    public int Level { get; set; }

    public Skill(string name, SkillDifficulty difficulty, int level = 0)
    {
        Name = name;
        Difficulty = difficulty;
        Level = Math.Max(0, Math.Min(10, level));
    }

    /// <summary>
    /// Calculates the cost to advance this skill to the next level
    /// </summary>
    public int AdvancementCost()
    {
        int baseCost = Level + 1;

        return Difficulty switch
        {
            SkillDifficulty.Easy => baseCost,
            SkillDifficulty.Average => baseCost * 2,
            SkillDifficulty.Hard => baseCost * 2,
            SkillDifficulty.VeryHard => baseCost * 3,
            _ => baseCost
        };
    }

    /// <summary>
    /// Advances the skill by one level if possible
    /// </summary>
    public bool Advance()
    {
        if (Level >= 10) return false;
        Level++;
        return true;
    }

    public override string ToString()
    {
        return $"{Name} ({Difficulty}): {Level}/10";
    }
}

/// <summary>
/// Manages skill progression and development
/// </summary>
public static class SkillSystem
{
    /// <summary>
    /// Calculates advancement cost for a skill at a given level
    /// </summary>
    public static int CalculateAdvancementCost(SkillDifficulty difficulty, int currentLevel)
    {
        int baseCost = currentLevel + 1;

        return difficulty switch
        {
            SkillDifficulty.Easy => baseCost,
            SkillDifficulty.Average => baseCost * 2,
            SkillDifficulty.Hard => baseCost * 2,
            SkillDifficulty.VeryHard => baseCost * 3,
            _ => baseCost
        };
    }

    /// <summary>
    /// Performs a skill check: skill level + d10 + modifiers vs difficulty class
    /// </summary>
    public static bool SkillCheck(int skillLevel, int difficultyClass, int modifiers = 0)
    {
        int roll = skillLevel + SteelkiltSharp.Core.Dice.D10() + modifiers;
        return roll >= difficultyClass;
    }
}
