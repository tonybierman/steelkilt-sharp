using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Modules;

public class ManeuversTests
{
    [Fact]
    public void ManeuverModifiers_Constructor_SetsProperties()
    {
        var modifiers = new ManeuverModifiers(2, -2, 3);

        Assert.Equal(2, modifiers.AttackBonus);
        Assert.Equal(-2, modifiers.DefenseBonus);
        Assert.Equal(3, modifiers.DamageBonus);
    }

    [Fact]
    public void GetModifiers_Normal_ReturnsZeroModifiers()
    {
        var modifiers = Maneuvers.GetModifiers(ManeuverType.Normal);

        Assert.Equal(0, modifiers.AttackBonus);
        Assert.Equal(0, modifiers.DefenseBonus);
        Assert.Equal(0, modifiers.DamageBonus);
    }

    [Fact]
    public void GetModifiers_Charge_ReturnsCorrectModifiers()
    {
        var modifiers = Maneuvers.GetModifiers(ManeuverType.Charge);

        Assert.Equal(2, modifiers.AttackBonus);
        Assert.Equal(-2, modifiers.DefenseBonus);
        Assert.Equal(2, modifiers.DamageBonus);
    }

    [Fact]
    public void GetModifiers_AllOutAttack_ReturnsCorrectModifiers()
    {
        var modifiers = Maneuvers.GetModifiers(ManeuverType.AllOutAttack);

        Assert.Equal(4, modifiers.AttackBonus);
        Assert.Equal(-4, modifiers.DefenseBonus);
        Assert.Equal(0, modifiers.DamageBonus);
    }

    [Fact]
    public void GetModifiers_DefensivePosition_ReturnsCorrectModifiers()
    {
        var modifiers = Maneuvers.GetModifiers(ManeuverType.DefensivePosition);

        Assert.Equal(-2, modifiers.AttackBonus);
        Assert.Equal(4, modifiers.DefenseBonus);
        Assert.Equal(0, modifiers.DamageBonus);
    }

    [Fact]
    public void GetDescription_Normal_ReturnsCorrectDescription()
    {
        string description = Maneuvers.GetDescription(ManeuverType.Normal);

        Assert.Contains("Normal", description);
    }

    [Fact]
    public void GetDescription_Charge_ReturnsCorrectDescription()
    {
        string description = Maneuvers.GetDescription(ManeuverType.Charge);

        Assert.Contains("Charge", description);
        Assert.Contains("+2 attack", description);
        Assert.Contains("-2 defense", description);
        Assert.Contains("+2 damage", description);
    }

    [Fact]
    public void GetDescription_AllOutAttack_ReturnsCorrectDescription()
    {
        string description = Maneuvers.GetDescription(ManeuverType.AllOutAttack);

        Assert.Contains("All-Out Attack", description);
        Assert.Contains("+4 attack", description);
        Assert.Contains("-4 defense", description);
    }

    [Fact]
    public void GetDescription_DefensivePosition_ReturnsCorrectDescription()
    {
        string description = Maneuvers.GetDescription(ManeuverType.DefensivePosition);

        Assert.Contains("Defensive Position", description);
        Assert.Contains("-2 attack", description);
        Assert.Contains("+4 defense", description);
    }

    [Fact]
    public void Maneuvers_AllTypesHaveModifiers()
    {
        foreach (ManeuverType maneuver in Enum.GetValues<ManeuverType>())
        {
            var modifiers = Maneuvers.GetModifiers(maneuver);
            Assert.NotNull(modifiers);
        }
    }

    [Fact]
    public void Maneuvers_AllTypesHaveDescriptions()
    {
        foreach (ManeuverType maneuver in Enum.GetValues<ManeuverType>())
        {
            string description = Maneuvers.GetDescription(maneuver);
            Assert.NotNull(description);
            Assert.NotEmpty(description);
        }
    }

    [Fact]
    public void ManeuverModifiers_PropertiesCanBeModified()
    {
        var modifiers = new ManeuverModifiers(1, 1, 1);

        modifiers.AttackBonus = 5;
        modifiers.DefenseBonus = -3;
        modifiers.DamageBonus = 2;

        Assert.Equal(5, modifiers.AttackBonus);
        Assert.Equal(-3, modifiers.DefenseBonus);
        Assert.Equal(2, modifiers.DamageBonus);
    }
}
