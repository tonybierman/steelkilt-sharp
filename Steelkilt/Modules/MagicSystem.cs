using System.ComponentModel;
using System.Runtime.CompilerServices;
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
/// Represents a magical spell with UI data-binding support
/// </summary>
public class Spell : INotifyPropertyChanged
{
    private string _name;
    private MagicBranch _branch;
    private SpellLevel _level;
    private int _castingTime;
    private int _exhaustionCost;
    private string _description;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public MagicBranch Branch
    {
        get => _branch;
        set => SetProperty(ref _branch, value);
    }

    public SpellLevel Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public int CastingTime
    {
        get => _castingTime;
        set => SetProperty(ref _castingTime, value);
    }

    public int ExhaustionCost
    {
        get => _exhaustionCost;
        set => SetProperty(ref _exhaustionCost, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public Spell(string name, MagicBranch branch, SpellLevel level, int castingTime, int exhaustionCost, string description)
    {
        _name = name;
        _branch = branch;
        _level = level;
        _castingTime = castingTime;
        _exhaustionCost = exhaustionCost;
        _description = description;
    }

    /// <summary>
    /// Calculates the difficulty class for casting this spell
    /// </summary>
    public int DifficultyClass => 10 + ((int)Level * 2);

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
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Represents a character's magical abilities with UI data-binding support
/// </summary>
public class MagicUser : INotifyPropertyChanged
{
    private Dictionary<MagicBranch, int> _branchSkills;
    private List<Spell> _preparedSpells;
    private int _magicalExhaustion;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Dictionary<MagicBranch, int> BranchSkills
    {
        get => _branchSkills;
        set => SetProperty(ref _branchSkills, value);
    }

    public List<Spell> PreparedSpells
    {
        get => _preparedSpells;
        set => SetProperty(ref _preparedSpells, value);
    }

    public int MagicalExhaustion
    {
        get => _magicalExhaustion;
        set => SetProperty(ref _magicalExhaustion, value);
    }

    public MagicUser()
    {
        _branchSkills = new Dictionary<MagicBranch, int>();
        _preparedSpells = new List<Spell>();
        _magicalExhaustion = 0;

        // Initialize all branches to 0
        foreach (MagicBranch branch in Enum.GetValues<MagicBranch>())
        {
            _branchSkills[branch] = 0;
        }
    }

    /// <summary>
    /// Sets skill level for a specific magic branch
    /// </summary>
    public void SetBranchSkill(MagicBranch branch, int skill)
    {
        BranchSkills[branch] = Math.Max(0, Math.Min(10, skill));
        OnPropertyChanged(nameof(BranchSkills));
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
            OnPropertyChanged(nameof(PreparedSpells));
        }
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
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Result of a spell casting attempt with UI data-binding support
/// </summary>
public class SpellCastResult : INotifyPropertyChanged
{
    private string _casterName;
    private Spell _spell;
    private int _castingRoll;
    private int _difficultyClass;
    private bool _success;
    private int _exhaustionGained;
    private string _message;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CasterName
    {
        get => _casterName;
        set => SetProperty(ref _casterName, value);
    }

    public Spell Spell
    {
        get => _spell;
        set => SetProperty(ref _spell, value);
    }

    public int CastingRoll
    {
        get => _castingRoll;
        set => SetProperty(ref _castingRoll, value);
    }

    public int DifficultyClass
    {
        get => _difficultyClass;
        set => SetProperty(ref _difficultyClass, value);
    }

    public bool Success
    {
        get => _success;
        set => SetProperty(ref _success, value);
    }

    public int ExhaustionGained
    {
        get => _exhaustionGained;
        set => SetProperty(ref _exhaustionGained, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public SpellCastResult(string casterName, Spell spell, int castingRoll, int difficultyClass, bool success, int exhaustionGained, string message)
    {
        _casterName = casterName;
        _spell = spell;
        _castingRoll = castingRoll;
        _difficultyClass = difficultyClass;
        _success = success;
        _exhaustionGained = exhaustionGained;
        _message = message;
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
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
