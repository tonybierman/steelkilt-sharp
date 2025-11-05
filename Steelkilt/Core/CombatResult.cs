using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteelkiltSharp.Core;

/// <summary>
/// Contains the results of a combat round
/// </summary>
public class CombatResult : INotifyPropertyChanged
{
    private string _attacker;
    private string _defender;
    private int _attackRoll;
    private int _defenseRoll;
    private bool _hit;
    private int _damage;
    private WoundLevel? _woundLevel;
    private bool _defenderDied;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Attacker
    {
        get => _attacker;
        set => SetProperty(ref _attacker, value);
    }

    public string Defender
    {
        get => _defender;
        set => SetProperty(ref _defender, value);
    }

    public int AttackRoll
    {
        get => _attackRoll;
        set => SetProperty(ref _attackRoll, value);
    }

    public int DefenseRoll
    {
        get => _defenseRoll;
        set => SetProperty(ref _defenseRoll, value);
    }

    public bool Hit
    {
        get => _hit;
        set => SetProperty(ref _hit, value);
    }

    public int Damage
    {
        get => _damage;
        set => SetProperty(ref _damage, value);
    }

    public WoundLevel? WoundLevel
    {
        get => _woundLevel;
        set => SetProperty(ref _woundLevel, value);
    }

    public bool DefenderDied
    {
        get => _defenderDied;
        set => SetProperty(ref _defenderDied, value);
    }

    public CombatResult(
        string attacker,
        string defender,
        int attackRoll,
        int defenseRoll,
        bool hit,
        int damage,
        WoundLevel? woundLevel,
        bool defenderDied)
    {
        _attacker = attacker;
        _defender = defender;
        _attackRoll = attackRoll;
        _defenseRoll = defenseRoll;
        _hit = hit;
        _damage = damage;
        _woundLevel = woundLevel;
        _defenderDied = defenderDied;
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
        if (!Hit)
        {
            return $"{Attacker} attacks {Defender}: MISS (Attack:{AttackRoll} vs Defense:{DefenseRoll})";
        }

        string woundStr = WoundLevel.HasValue ? $" - {WoundLevel.Value} wound" : "";
        string deathStr = DefenderDied ? " - DEFENDER DIED!" : "";

        return $"{Attacker} attacks {Defender}: HIT for {Damage} damage" +
               $" (Attack:{AttackRoll} vs Defense:{DefenseRoll}){woundStr}{deathStr}";
    }
}
