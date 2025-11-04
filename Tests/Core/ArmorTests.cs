using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class ArmorTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var armor = new Armor("Test Armor", ArmorType.Chain, 3, -1);

        Assert.Equal("Test Armor", armor.Name);
        Assert.Equal(ArmorType.Chain, armor.ArmorType);
        Assert.Equal(3, armor.Protection);
        Assert.Equal(-1, armor.MovementPenalty);
    }

    [Fact]
    public void None_CreatesNoArmorWithZeroValues()
    {
        var armor = Armor.None();

        Assert.Equal("None", armor.Name);
        Assert.Equal(0, armor.Protection);
        Assert.Equal(0, armor.MovementPenalty);
    }

    [Fact]
    public void HeavyCloth_CreatesCorrectArmor()
    {
        var armor = Armor.HeavyCloth();

        Assert.Equal("Heavy Cloth", armor.Name);
        Assert.Equal(ArmorType.HeavyCloth, armor.ArmorType);
        Assert.Equal(1, armor.Protection);
        Assert.Equal(0, armor.MovementPenalty);
    }

    [Fact]
    public void Leather_CreatesCorrectArmor()
    {
        var armor = Armor.Leather();

        Assert.Equal("Leather", armor.Name);
        Assert.Equal(ArmorType.Leather, armor.ArmorType);
        Assert.Equal(2, armor.Protection);
        Assert.Equal(0, armor.MovementPenalty);
    }

    [Fact]
    public void Chain_CreatesCorrectArmor()
    {
        var armor = Armor.Chain();

        Assert.Equal("Chain Mail", armor.Name);
        Assert.Equal(ArmorType.Chain, armor.ArmorType);
        Assert.Equal(3, armor.Protection);
        Assert.Equal(-1, armor.MovementPenalty);
    }

    [Fact]
    public void Plate_CreatesCorrectArmor()
    {
        var armor = Armor.Plate();

        Assert.Equal("Plate Armor", armor.Name);
        Assert.Equal(ArmorType.Plate, armor.ArmorType);
        Assert.Equal(4, armor.Protection);
        Assert.Equal(-2, armor.MovementPenalty);
    }

    [Fact]
    public void FullPlate_CreatesCorrectArmor()
    {
        var armor = Armor.FullPlate();

        Assert.Equal("Full Plate", armor.Name);
        Assert.Equal(ArmorType.FullPlate, armor.ArmorType);
        Assert.Equal(5, armor.Protection);
        Assert.Equal(-3, armor.MovementPenalty);
    }

    [Fact]
    public void Armor_PropertiesCanBeModified()
    {
        var armor = Armor.Leather();
        armor.Name = "Studded Leather";
        armor.Protection = 3;
        armor.MovementPenalty = -1;

        Assert.Equal("Studded Leather", armor.Name);
        Assert.Equal(3, armor.Protection);
        Assert.Equal(-1, armor.MovementPenalty);
    }
}
