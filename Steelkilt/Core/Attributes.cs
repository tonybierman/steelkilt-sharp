using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteelkiltSharp.Core;

/// <summary>
/// Represents the nine core attributes of a character in the Draft RPG system.
/// Attributes are organized into Physical, Mental, and Interactive categories.
/// All values are clamped between 1 and 10.
/// </summary>
public class Attributes : INotifyPropertyChanged
{
    private int _strength;
    private int _dexterity;
    private int _constitution;
    private int _reason;
    private int _intuition;
    private int _willpower;
    private int _charisma;
    private int _perception;
    private int _empathy;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Physical: Strength attribute (1-10)</summary>
    public int Strength
    {
        get => _strength;
        set => SetProperty(ref _strength, Clamp(value));
    }

    /// <summary>Physical: Dexterity attribute (1-10)</summary>
    public int Dexterity
    {
        get => _dexterity;
        set => SetProperty(ref _dexterity, Clamp(value));
    }

    /// <summary>Physical: Constitution attribute (1-10)</summary>
    public int Constitution
    {
        get => _constitution;
        set => SetProperty(ref _constitution, Clamp(value));
    }

    /// <summary>Mental: Reason attribute (1-10)</summary>
    public int Reason
    {
        get => _reason;
        set => SetProperty(ref _reason, Clamp(value));
    }

    /// <summary>Mental: Intuition attribute (1-10)</summary>
    public int Intuition
    {
        get => _intuition;
        set => SetProperty(ref _intuition, Clamp(value));
    }

    /// <summary>Mental: Willpower attribute (1-10)</summary>
    public int Willpower
    {
        get => _willpower;
        set => SetProperty(ref _willpower, Clamp(value));
    }

    /// <summary>Interactive: Charisma attribute (1-10)</summary>
    public int Charisma
    {
        get => _charisma;
        set => SetProperty(ref _charisma, Clamp(value));
    }

    /// <summary>Interactive: Perception attribute (1-10)</summary>
    public int Perception
    {
        get => _perception;
        set => SetProperty(ref _perception, Clamp(value));
    }

    /// <summary>Interactive: Empathy attribute (1-10)</summary>
    public int Empathy
    {
        get => _empathy;
        set => SetProperty(ref _empathy, Clamp(value));
    }

    /// <summary>
    /// Calculates stamina as (Strength + Constitution) / 2
    /// </summary>
    public int Stamina => (Strength + Constitution) / 2;

    public Attributes(
        int strength,
        int dexterity,
        int constitution,
        int reason,
        int intuition,
        int willpower,
        int charisma,
        int perception,
        int empathy)
    {
        _strength = Clamp(strength);
        _dexterity = Clamp(dexterity);
        _constitution = Clamp(constitution);
        _reason = Clamp(reason);
        _intuition = Clamp(intuition);
        _willpower = Clamp(willpower);
        _charisma = Clamp(charisma);
        _perception = Clamp(perception);
        _empathy = Clamp(empathy);
    }

    /// <summary>
    /// Clamps a value between 1 and 10
    /// </summary>
    private static int Clamp(int value)
    {
        return Math.Max(1, Math.Min(10, value));
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
}
