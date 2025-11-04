using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class AttributesTests
{
    [Fact]
    public void Constructor_SetsAllAttributes()
    {
        var attrs = new Attributes(8, 7, 6, 5, 4, 3, 2, 1, 9);

        Assert.Equal(8, attrs.Strength);
        Assert.Equal(7, attrs.Dexterity);
        Assert.Equal(6, attrs.Constitution);
        Assert.Equal(5, attrs.Reason);
        Assert.Equal(4, attrs.Intuition);
        Assert.Equal(3, attrs.Willpower);
        Assert.Equal(2, attrs.Charisma);
        Assert.Equal(1, attrs.Perception);
        Assert.Equal(9, attrs.Empathy);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(11, 10)]
    [InlineData(15, 10)]
    public void Constructor_ClampsAttributesToValidRange(int input, int expected)
    {
        var attrs = new Attributes(input, input, input, input, input, input, input, input, input);

        Assert.Equal(expected, attrs.Strength);
        Assert.Equal(expected, attrs.Dexterity);
        Assert.Equal(expected, attrs.Constitution);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Constructor_AcceptsValidAttributes(int value)
    {
        var attrs = new Attributes(value, value, value, value, value, value, value, value, value);

        Assert.Equal(value, attrs.Strength);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(11, 10)]
    [InlineData(20, 10)]
    public void Strength_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Strength = input;

        Assert.Equal(expected, attrs.Strength);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Dexterity_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Dexterity = input;

        Assert.Equal(expected, attrs.Dexterity);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Constitution_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Constitution = input;

        Assert.Equal(expected, attrs.Constitution);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Reason_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Reason = input;

        Assert.Equal(expected, attrs.Reason);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Intuition_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Intuition = input;

        Assert.Equal(expected, attrs.Intuition);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Willpower_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Willpower = input;

        Assert.Equal(expected, attrs.Willpower);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Charisma_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Charisma = input;

        Assert.Equal(expected, attrs.Charisma);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Perception_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Perception = input;

        Assert.Equal(expected, attrs.Perception);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(11, 10)]
    public void Empathy_Setter_ClampsValue(int input, int expected)
    {
        var attrs = new Attributes(5, 5, 5, 5, 5, 5, 5, 5, 5);
        attrs.Empathy = input;

        Assert.Equal(expected, attrs.Empathy);
    }

    [Theory]
    [InlineData(8, 8, 8)]
    [InlineData(6, 4, 5)]
    [InlineData(10, 10, 10)]
    [InlineData(1, 1, 1)]
    [InlineData(5, 3, 4)]
    public void Stamina_CalculatesCorrectly(int strength, int constitution, int expectedStamina)
    {
        var attrs = new Attributes(strength, 5, constitution, 5, 5, 5, 5, 5, 5);

        Assert.Equal(expectedStamina, attrs.Stamina);
    }
}
