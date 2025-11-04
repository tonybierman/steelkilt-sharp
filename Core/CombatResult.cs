namespace SteelkiltSharp.Core;

/// <summary>
/// Contains the results of a combat round
/// </summary>
public class CombatResult
{
    public string Attacker { get; set; }
    public string Defender { get; set; }
    public int AttackRoll { get; set; }
    public int DefenseRoll { get; set; }
    public bool Hit { get; set; }
    public int Damage { get; set; }
    public WoundLevel? WoundLevel { get; set; }
    public bool DefenderDied { get; set; }

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
        Attacker = attacker;
        Defender = defender;
        AttackRoll = attackRoll;
        DefenseRoll = defenseRoll;
        Hit = hit;
        Damage = damage;
        WoundLevel = woundLevel;
        DefenderDied = defenderDied;
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
