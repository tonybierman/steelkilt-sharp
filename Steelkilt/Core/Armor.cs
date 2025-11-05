using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteelkiltSharp.Core;

/// <summary>
/// Represents armor types with protection values
/// </summary>
public enum ArmorType
{
    HeavyCloth = 1,
    Leather = 2,
    Chain = 3,
    Plate = 4,
    FullPlate = 5
}

/// <summary>
/// Represents armor with protection and movement penalty
/// </summary>
public class Armor : INotifyPropertyChanged
{
    private string _name;
    private ArmorType _armorType;
    private int _protection;
    private int _movementPenalty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ArmorType ArmorType
    {
        get => _armorType;
        set => SetProperty(ref _armorType, value);
    }

    public int Protection
    {
        get => _protection;
        set => SetProperty(ref _protection, value);
    }

    public int MovementPenalty
    {
        get => _movementPenalty;
        set => SetProperty(ref _movementPenalty, value);
    }

    public Armor(string name, ArmorType armorType, int protection, int movementPenalty)
    {
        _name = name;
        _armorType = armorType;
        _protection = protection;
        _movementPenalty = movementPenalty;
    }

    /// <summary>
    /// Creates no armor (0 protection, 0 penalty)
    /// </summary>
    public static Armor None() => new("None", ArmorType.HeavyCloth, 0, 0);

    /// <summary>
    /// Creates heavy cloth armor
    /// </summary>
    public static Armor HeavyCloth() => new("Heavy Cloth", ArmorType.HeavyCloth, 1, 0);

    /// <summary>
    /// Creates leather armor
    /// </summary>
    public static Armor Leather() => new("Leather", ArmorType.Leather, 2, 0);

    /// <summary>
    /// Creates chain mail armor
    /// </summary>
    public static Armor Chain() => new("Chain Mail", ArmorType.Chain, 3, -1);

    /// <summary>
    /// Creates plate armor
    /// </summary>
    public static Armor Plate() => new("Plate Armor", ArmorType.Plate, 4, -2);

    /// <summary>
    /// Creates full plate armor
    /// </summary>
    public static Armor FullPlate() => new("Full Plate", ArmorType.FullPlate, 5, -3);

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
