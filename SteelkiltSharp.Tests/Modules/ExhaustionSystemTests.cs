using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Modules;

public class ExhaustionSystemTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(-5, 0)]
    [InlineData(1, -1)]
    [InlineData(5, -1)]
    [InlineData(6, -2)]
    [InlineData(10, -2)]
    [InlineData(11, -3)]
    [InlineData(15, -3)]
    [InlineData(16, -4)]
    [InlineData(20, -4)]
    [InlineData(100, -4)]
    public void GetPenalty_ReturnsCorrectValue(int exhaustion, int expectedPenalty)
    {
        int penalty = ExhaustionSystem.GetPenalty(exhaustion);

        Assert.Equal(expectedPenalty, penalty);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(5, 1, 6)]
    [InlineData(10, 3, 13)]
    [InlineData(15, 5, 20)]
    public void AddCombatExhaustion_IncreasesExhaustion(int current, int amount, int expected)
    {
        int result = ExhaustionSystem.AddCombatExhaustion(current, amount);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddCombatExhaustion_DefaultAmount_AddsOne()
    {
        int result = ExhaustionSystem.AddCombatExhaustion(5);

        Assert.Equal(6, result);
    }

    [Theory]
    [InlineData(10, 1, 9)]
    [InlineData(5, 3, 2)]
    [InlineData(15, 10, 5)]
    [InlineData(3, 5, 0)]
    public void RecoverExhaustion_DecreasesExhaustion(int current, int amount, int expected)
    {
        int result = ExhaustionSystem.RecoverExhaustion(current, amount);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RecoverExhaustion_CannotGoBelowZero()
    {
        int result = ExhaustionSystem.RecoverExhaustion(3, 10);

        Assert.Equal(0, result);
    }

    [Fact]
    public void RecoverExhaustion_DefaultAmount_RemovesOne()
    {
        int result = ExhaustionSystem.RecoverExhaustion(5);

        Assert.Equal(4, result);
    }

    [Theory]
    [InlineData(0, "Fresh")]
    [InlineData(-1, "Fresh")]
    [InlineData(1, "Slightly Tired (-1)")]
    [InlineData(5, "Slightly Tired (-1)")]
    [InlineData(6, "Tired (-2)")]
    [InlineData(10, "Tired (-2)")]
    [InlineData(11, "Exhausted (-3)")]
    [InlineData(15, "Exhausted (-3)")]
    [InlineData(16, "Completely Exhausted (-4)")]
    [InlineData(20, "Completely Exhausted (-4)")]
    [InlineData(100, "Completely Exhausted (-4)")]
    public void GetExhaustionDescription_ReturnsCorrectDescription(int exhaustion, string expectedDescription)
    {
        string description = ExhaustionSystem.GetExhaustionDescription(exhaustion);

        Assert.Equal(expectedDescription, description);
    }

    [Fact]
    public void ExhaustionSystem_ProgressionThroughLevels()
    {
        int exhaustion = 0;

        // Fresh
        Assert.Equal(0, ExhaustionSystem.GetPenalty(exhaustion));
        Assert.Equal("Fresh", ExhaustionSystem.GetExhaustionDescription(exhaustion));

        // Add exhaustion to Slightly Tired
        exhaustion = ExhaustionSystem.AddCombatExhaustion(exhaustion, 3);
        Assert.Equal(-1, ExhaustionSystem.GetPenalty(exhaustion));
        Assert.Contains("Slightly Tired", ExhaustionSystem.GetExhaustionDescription(exhaustion));

        // Add more to Tired
        exhaustion = ExhaustionSystem.AddCombatExhaustion(exhaustion, 5);
        Assert.Equal(-2, ExhaustionSystem.GetPenalty(exhaustion));
        Assert.Contains("Tired", ExhaustionSystem.GetExhaustionDescription(exhaustion));

        // Add more to Exhausted
        exhaustion = ExhaustionSystem.AddCombatExhaustion(exhaustion, 5);
        Assert.Equal(-3, ExhaustionSystem.GetPenalty(exhaustion));
        Assert.Contains("Exhausted", ExhaustionSystem.GetExhaustionDescription(exhaustion));

        // Add more to Completely Exhausted
        exhaustion = ExhaustionSystem.AddCombatExhaustion(exhaustion, 5);
        Assert.Equal(-4, ExhaustionSystem.GetPenalty(exhaustion));
        Assert.Contains("Completely Exhausted", ExhaustionSystem.GetExhaustionDescription(exhaustion));

        // Recover back down
        exhaustion = ExhaustionSystem.RecoverExhaustion(exhaustion, 10);
        Assert.Equal(-2, ExhaustionSystem.GetPenalty(exhaustion));
    }
}
