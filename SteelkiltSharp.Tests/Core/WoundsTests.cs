using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class WoundsTests
{
    [Fact]
    public void Constructor_InitializesWithNoWounds()
    {
        var wounds = new Wounds();

        Assert.Equal(0, wounds.Light);
        Assert.Equal(0, wounds.Severe);
        Assert.Equal(0, wounds.Critical);
        Assert.False(wounds.IsDead);
    }

    [Fact]
    public void AddWound_Light_IncrementsLightWound()
    {
        var wounds = new Wounds();
        wounds.AddWound(WoundLevel.Light);

        Assert.Equal(1, wounds.Light);
        Assert.Equal(0, wounds.Severe);
        Assert.Equal(0, wounds.Critical);
    }

    [Fact]
    public void AddWound_Severe_IncrementsSevereWound()
    {
        var wounds = new Wounds();
        wounds.AddWound(WoundLevel.Severe);

        Assert.Equal(0, wounds.Light);
        Assert.Equal(1, wounds.Severe);
        Assert.Equal(0, wounds.Critical);
    }

    [Fact]
    public void AddWound_Critical_IncrementsCriticalWound()
    {
        var wounds = new Wounds();
        wounds.AddWound(WoundLevel.Critical);

        Assert.Equal(0, wounds.Light);
        Assert.Equal(0, wounds.Severe);
        Assert.Equal(1, wounds.Critical);
    }

    [Fact]
    public void AddWound_FourLightWounds_ConvertsToOneSevere()
    {
        var wounds = new Wounds();

        wounds.AddWound(WoundLevel.Light);
        wounds.AddWound(WoundLevel.Light);
        wounds.AddWound(WoundLevel.Light);
        wounds.AddWound(WoundLevel.Light);

        Assert.Equal(0, wounds.Light);
        Assert.Equal(1, wounds.Severe);
        Assert.Equal(0, wounds.Critical);
    }

    [Fact]
    public void AddWound_ThreeSevereWounds_ConvertsToOneCritical()
    {
        var wounds = new Wounds();

        wounds.AddWound(WoundLevel.Severe);
        wounds.AddWound(WoundLevel.Severe);
        wounds.AddWound(WoundLevel.Severe);

        Assert.Equal(0, wounds.Light);
        Assert.Equal(0, wounds.Severe);
        Assert.Equal(1, wounds.Critical);
    }

    [Fact]
    public void AddWound_ComplexStacking_ConvertsCorrectly()
    {
        var wounds = new Wounds();

        // Add 5 light wounds: should become 1 severe + 1 light
        for (int i = 0; i < 5; i++)
        {
            wounds.AddWound(WoundLevel.Light);
        }

        Assert.Equal(1, wounds.Light);
        Assert.Equal(1, wounds.Severe);
    }

    [Fact]
    public void AddWound_SevereWoundsStackingToCritical()
    {
        var wounds = new Wounds();

        // Add 7 severe wounds: should become 2 critical + 1 severe
        for (int i = 0; i < 7; i++)
        {
            wounds.AddWound(WoundLevel.Severe);
        }

        Assert.Equal(0, wounds.Light);
        Assert.Equal(1, wounds.Severe);
        Assert.Equal(2, wounds.Critical);
    }

    [Fact]
    public void IsDead_TwoCriticalWounds_ReturnsTrue()
    {
        var wounds = new Wounds();

        wounds.AddWound(WoundLevel.Critical);
        Assert.False(wounds.IsDead);

        wounds.AddWound(WoundLevel.Critical);
        Assert.True(wounds.IsDead);
    }

    [Fact]
    public void IsDead_LessThanTwoCritical_ReturnsFalse()
    {
        var wounds = new Wounds();

        wounds.AddWound(WoundLevel.Light);
        wounds.AddWound(WoundLevel.Severe);
        wounds.AddWound(WoundLevel.Critical);

        Assert.False(wounds.IsDead);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, -1)]
    [InlineData(0, 1, 0, -2)]
    [InlineData(0, 0, 1, -4)]
    [InlineData(2, 1, 0, -4)]
    [InlineData(1, 1, 1, -7)]
    public void TotalPenalty_CalculatesCorrectly(int light, int severe, int critical, int expectedPenalty)
    {
        var wounds = new Wounds();

        for (int i = 0; i < light; i++) wounds.AddWound(WoundLevel.Light);
        for (int i = 0; i < severe; i++) wounds.AddWound(WoundLevel.Severe);
        for (int i = 0; i < critical; i++) wounds.AddWound(WoundLevel.Critical);

        Assert.Equal(expectedPenalty, wounds.TotalPenalty);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, -1)]
    [InlineData(0, 1, 0, -2)]
    [InlineData(0, 0, 1, -3)]
    [InlineData(3, 2, 1, -10)]
    public void MovementPenalty_CalculatesCorrectly(int light, int severe, int critical, int expectedPenalty)
    {
        var wounds = new Wounds();

        for (int i = 0; i < light; i++) wounds.AddWound(WoundLevel.Light);
        for (int i = 0; i < severe; i++) wounds.AddWound(WoundLevel.Severe);
        for (int i = 0; i < critical; i++) wounds.AddWound(WoundLevel.Critical);

        Assert.Equal(expectedPenalty, wounds.MovementPenalty);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var wounds = new Wounds();
        wounds.AddWound(WoundLevel.Light);
        wounds.AddWound(WoundLevel.Severe);
        wounds.AddWound(WoundLevel.Critical);

        string result = wounds.ToString();

        Assert.Contains("Light: 1", result);
        Assert.Contains("Severe: 1", result);
        Assert.Contains("Critical: 1", result);
    }
}
