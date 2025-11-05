using System.ComponentModel;
using System.Runtime.CompilerServices;

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
/// Represents a learnable skill with progression mechanics and UI data-binding support
/// </summary>
public class Skill : INotifyPropertyChanged
{
    private string _name;
    private SkillDifficulty _difficulty;
    private int _level;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public SkillDifficulty Difficulty
    {
        get => _difficulty;
        set => SetProperty(ref _difficulty, value);
    }

    public int Level
    {
        get => _level;
        set => SetProperty(ref _level, Math.Max(0, Math.Min(10, value)));
    }

    public Skill(string name, SkillDifficulty difficulty, int level = 0)
    {
        _name = name;
        _difficulty = difficulty;
        _level = Math.Max(0, Math.Min(10, level));
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
