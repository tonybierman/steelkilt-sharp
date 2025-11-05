using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteelkiltSharp.Core;

/// <summary>
/// Represents weapon impact classes in the Draft RPG system
/// </summary>
public enum WeaponImpact
{
    Small = 1,
    Medium = 2,
    Large = 3,
    Huge = 4
}

/// <summary>
/// Represents a weapon with its combat characteristics
/// </summary>
public class Weapon : INotifyPropertyChanged
{
    private string _name;
    private WeaponImpact _impact;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public WeaponImpact Impact
    {
        get => _impact;
        set => SetProperty(ref _impact, value);
    }

    /// <summary>
    /// Base damage calculated as (impact × 2) + 1
    /// </summary>
    public int Damage => ((int)Impact * 2) + 1;

    public Weapon(string name, WeaponImpact impact)
    {
        _name = name;
        _impact = impact;
    }

    /// <summary>
    /// Creates a dagger (Small impact)
    /// </summary>
    public static Weapon Dagger() => new("Dagger", WeaponImpact.Small);

    /// <summary>
    /// Creates a long sword (Medium impact)
    /// </summary>
    public static Weapon LongSword() => new("Long Sword", WeaponImpact.Medium);

    /// <summary>
    /// Creates a two-handed sword (Large impact)
    /// </summary>
    public static Weapon TwoHandedSword() => new("Two-Handed Sword", WeaponImpact.Large);

    /// <summary>
    /// Creates a great axe (Huge impact)
    /// </summary>
    public static Weapon GreatAxe() => new("Great Axe", WeaponImpact.Huge);

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
