using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class DiceTests
{
    [Fact]
    public void D10_ReturnsValueInValidRange()
    {
        for (int i = 0; i < 100; i++)
        {
            int result = Dice.D10();
            Assert.InRange(result, 1, 10);
        }
    }

    [Fact]
    public void D10_ProducesDifferentValues()
    {
        var results = new HashSet<int>();

        // Roll 50 times, should get at least 3 different values
        for (int i = 0; i < 50; i++)
        {
            results.Add(Dice.D10());
        }

        Assert.True(results.Count >= 3, "D10 should produce varied results");
    }

    [Theory]
    [InlineData(1, 1, 10)]
    [InlineData(2, 2, 20)]
    [InlineData(3, 3, 30)]
    [InlineData(5, 5, 50)]
    public void RollMultiple_ReturnsValidRange(int count, int min, int max)
    {
        for (int i = 0; i < 20; i++)
        {
            int result = Dice.RollMultiple(count);
            Assert.InRange(result, min, max);
        }
    }

    [Fact]
    public void RollMultiple_ZeroCount_ReturnsZero()
    {
        int result = Dice.RollMultiple(0);
        Assert.Equal(0, result);
    }

    [Fact]
    public void RollMultiple_ProducesVariedResults()
    {
        var results = new HashSet<int>();

        for (int i = 0; i < 30; i++)
        {
            results.Add(Dice.RollMultiple(3));
        }

        // Rolling 3d10 30 times should produce at least 5 different values
        Assert.True(results.Count >= 5, "RollMultiple should produce varied results");
    }
}
