using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class WeaponTests
{
    [Theory]
    [InlineData(WeaponImpact.Small, 3)]
    [InlineData(WeaponImpact.Medium, 5)]
    [InlineData(WeaponImpact.Large, 7)]
    [InlineData(WeaponImpact.Huge, 9)]
    public void Damage_CalculatesCorrectly(WeaponImpact impact, int expectedDamage)
    {
        var weapon = new Weapon("Test Weapon", impact);

        Assert.Equal(expectedDamage, weapon.Damage);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var weapon = new Weapon("Battle Axe", WeaponImpact.Large);

        Assert.Equal("Battle Axe", weapon.Name);
        Assert.Equal(WeaponImpact.Large, weapon.Impact);
    }

    [Fact]
    public void Dagger_CreatesSmallImpactWeapon()
    {
        var dagger = Weapon.Dagger();

        Assert.Equal("Dagger", dagger.Name);
        Assert.Equal(WeaponImpact.Small, dagger.Impact);
        Assert.Equal(3, dagger.Damage);
    }

    [Fact]
    public void LongSword_CreatesMediumImpactWeapon()
    {
        var sword = Weapon.LongSword();

        Assert.Equal("Long Sword", sword.Name);
        Assert.Equal(WeaponImpact.Medium, sword.Impact);
        Assert.Equal(5, sword.Damage);
    }

    [Fact]
    public void TwoHandedSword_CreatesLargeImpactWeapon()
    {
        var sword = Weapon.TwoHandedSword();

        Assert.Equal("Two-Handed Sword", sword.Name);
        Assert.Equal(WeaponImpact.Large, sword.Impact);
        Assert.Equal(7, sword.Damage);
    }

    [Fact]
    public void GreatAxe_CreatesHugeImpactWeapon()
    {
        var axe = Weapon.GreatAxe();

        Assert.Equal("Great Axe", axe.Name);
        Assert.Equal(WeaponImpact.Huge, axe.Impact);
        Assert.Equal(9, axe.Damage);
    }

    [Fact]
    public void Weapon_PropertiesCanBeModified()
    {
        var weapon = Weapon.Dagger();
        weapon.Name = "Enchanted Dagger";
        weapon.Impact = WeaponImpact.Medium;

        Assert.Equal("Enchanted Dagger", weapon.Name);
        Assert.Equal(WeaponImpact.Medium, weapon.Impact);
        Assert.Equal(5, weapon.Damage);
    }
}
