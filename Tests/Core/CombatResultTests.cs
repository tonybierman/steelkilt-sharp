using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class CombatResultTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var result = new CombatResult(
            "Attacker",
            "Defender",
            15,
            10,
            true,
            5,
            WoundLevel.Light,
            false);

        Assert.Equal("Attacker", result.Attacker);
        Assert.Equal("Defender", result.Defender);
        Assert.Equal(15, result.AttackRoll);
        Assert.Equal(10, result.DefenseRoll);
        Assert.True(result.Hit);
        Assert.Equal(5, result.Damage);
        Assert.Equal(WoundLevel.Light, result.WoundLevel);
        Assert.False(result.DefenderDied);
    }

    [Fact]
    public void ToString_Miss_ReturnsCorrectFormat()
    {
        var result = new CombatResult(
            "Hero",
            "Goblin",
            12,
            15,
            false,
            0,
            null,
            false);

        string output = result.ToString();

        Assert.Contains("Hero attacks Goblin", output);
        Assert.Contains("MISS", output);
        Assert.Contains("Attack:12", output);
        Assert.Contains("Defense:15", output);
    }

    [Fact]
    public void ToString_Hit_ReturnsCorrectFormat()
    {
        var result = new CombatResult(
            "Knight",
            "Dragon",
            18,
            12,
            true,
            6,
            WoundLevel.Severe,
            false);

        string output = result.ToString();

        Assert.Contains("Knight attacks Dragon", output);
        Assert.Contains("HIT", output);
        Assert.Contains("6 damage", output);
        Assert.Contains("Attack:18", output);
        Assert.Contains("Defense:12", output);
        Assert.Contains("Severe wound", output);
    }

    [Fact]
    public void ToString_HitWithDeath_IncludesDeathMessage()
    {
        var result = new CombatResult(
            "Warrior",
            "Enemy",
            20,
            10,
            true,
            12,
            WoundLevel.Critical,
            true);

        string output = result.ToString();

        Assert.Contains("DEFENDER DIED!", output);
    }

    [Fact]
    public void ToString_HitWithNoDamage_ShowsZeroDamage()
    {
        var result = new CombatResult(
            "Attacker",
            "Defender",
            14,
            13,
            true,
            0,
            null,
            false);

        string output = result.ToString();

        Assert.Contains("HIT for 0 damage", output);
    }

    [Fact]
    public void CombatResult_SupportsNullWoundLevel()
    {
        var result = new CombatResult(
            "A",
            "D",
            10,
            8,
            true,
            0,
            null,
            false);

        Assert.Null(result.WoundLevel);
        string output = result.ToString();
        Assert.DoesNotContain("wound", output, StringComparison.OrdinalIgnoreCase);
    }
}
