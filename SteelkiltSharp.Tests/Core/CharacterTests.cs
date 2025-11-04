using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Core;

public class CharacterTests
{
    private Character CreateTestCharacter(int strength = 7, int weaponSkill = 6, int dodgeSkill = 5)
    {
        return new Character(
            "Test Character",
            new Attributes(strength, 7, 6, 5, 5, 5, 5, 5, 5),
            weaponSkill,
            dodgeSkill,
            Weapon.LongSword(),
            Armor.Leather());
    }

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var attrs = new Attributes(8, 7, 6, 5, 4, 3, 2, 1, 9);
        var weapon = Weapon.LongSword();
        var armor = Armor.Chain();

        var character = new Character("Hero", attrs, 7, 5, weapon, armor);

        Assert.Equal("Hero", character.Name);
        Assert.Equal(attrs, character.Attributes);
        Assert.Equal(7, character.WeaponSkill);
        Assert.Equal(5, character.DodgeSkill);
        Assert.Equal(weapon, character.Weapon);
        Assert.Equal(armor, character.Armor);
        Assert.NotNull(character.Wounds);
        Assert.Equal(0, character.Exhaustion);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    [InlineData(15, 10)]
    public void Constructor_ClampsWeaponSkill(int input, int expected)
    {
        var character = CreateTestCharacter(weaponSkill: input);

        Assert.Equal(expected, character.WeaponSkill);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void Constructor_ClampsDodgeSkill(int input, int expected)
    {
        var character = CreateTestCharacter(dodgeSkill: input);

        Assert.Equal(expected, character.DodgeSkill);
    }

    [Theory]
    [InlineData(1, -1)]
    [InlineData(2, -1)]
    [InlineData(3, 0)]
    [InlineData(6, 0)]
    [InlineData(7, 1)]
    [InlineData(8, 1)]
    [InlineData(9, 2)]
    [InlineData(10, 2)]
    public void StrengthBonus_CalculatesCorrectly(int strength, int expectedBonus)
    {
        var character = CreateTestCharacter(strength: strength);

        Assert.Equal(expectedBonus, character.StrengthBonus);
    }

    [Fact]
    public void AttackRoll_ReturnsValueInValidRange()
    {
        var character = CreateTestCharacter(weaponSkill: 7);

        for (int i = 0; i < 20; i++)
        {
            int roll = character.AttackRoll();
            // WeaponSkill 7 + d10 (1-10) = 8-17 at minimum
            Assert.InRange(roll, 8, 17);
        }
    }

    [Fact]
    public void AttackRoll_IncludesWoundPenalty()
    {
        var character = CreateTestCharacter(weaponSkill: 10);
        character.Wounds.AddWound(WoundLevel.Severe);

        // With max weapon skill (10) and a severe wound (-2), max roll is 10 + 10 - 2 = 18
        for (int i = 0; i < 20; i++)
        {
            int roll = character.AttackRoll();
            Assert.True(roll <= 18);
        }
    }

    [Fact]
    public void AttackRoll_IncludesArmorPenalty()
    {
        var character = CreateTestCharacter(weaponSkill: 10);
        character.Armor = Armor.FullPlate(); // -3 penalty

        // With max weapon skill (10), full plate (-3), max roll is 10 + 10 - 3 = 17
        for (int i = 0; i < 20; i++)
        {
            int roll = character.AttackRoll();
            Assert.True(roll <= 17);
        }
    }

    [Fact]
    public void AttackRoll_IncludesExhaustionPenalty()
    {
        var character = CreateTestCharacter(weaponSkill: 10);
        character.Exhaustion = 6; // -2 penalty

        // With max weapon skill (10), exhaustion (-2), max roll is 10 + 10 - 2 = 18
        for (int i = 0; i < 20; i++)
        {
            int roll = character.AttackRoll();
            Assert.True(roll <= 18);
        }
    }

    [Fact]
    public void ParryRoll_ReturnsValueInValidRange()
    {
        var character = CreateTestCharacter(weaponSkill: 7);

        for (int i = 0; i < 20; i++)
        {
            int roll = character.ParryRoll();
            Assert.InRange(roll, 8, 17);
        }
    }

    [Fact]
    public void ParryRoll_IncludesAllModifiers()
    {
        var character = CreateTestCharacter(weaponSkill: 10);
        character.Wounds.AddWound(WoundLevel.Light); // -1
        character.Armor = Armor.Chain(); // -1
        character.Exhaustion = 3; // -1

        // Max roll: 10 + 10 - 1 - 1 - 1 = 17
        for (int i = 0; i < 20; i++)
        {
            int roll = character.ParryRoll();
            Assert.True(roll <= 17);
        }
    }

    [Fact]
    public void DodgeRoll_ReturnsValueInValidRange()
    {
        var character = CreateTestCharacter(dodgeSkill: 6);

        for (int i = 0; i < 20; i++)
        {
            int roll = character.DodgeRoll();
            Assert.InRange(roll, 7, 16);
        }
    }

    [Fact]
    public void DodgeRoll_IncludesAllModifiers()
    {
        var character = CreateTestCharacter(dodgeSkill: 10);
        character.Wounds.AddWound(WoundLevel.Critical); // -4
        character.Armor = Armor.Plate(); // -2
        character.Exhaustion = 11; // -3

        // Max roll: 10 + 10 - 4 - 2 - 3 = 11
        for (int i = 0; i < 20; i++)
        {
            int roll = character.DodgeRoll();
            Assert.True(roll <= 11);
        }
    }

    [Fact]
    public void IsDead_WithLessThanTwoCriticalWounds_ReturnsFalse()
    {
        var character = CreateTestCharacter();
        character.Wounds.AddWound(WoundLevel.Critical);

        Assert.False(character.IsDead);
    }

    [Fact]
    public void IsDead_WithTwoCriticalWounds_ReturnsTrue()
    {
        var character = CreateTestCharacter();
        character.Wounds.AddWound(WoundLevel.Critical);
        character.Wounds.AddWound(WoundLevel.Critical);

        Assert.True(character.IsDead);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var character = CreateTestCharacter();

        string result = character.ToString();

        Assert.Contains("Test Character", result);
        Assert.Contains("STR:7", result);
        Assert.Contains("WeaponSkill:6", result);
        Assert.Contains("DodgeSkill:5", result);
    }

    [Fact]
    public void Character_SupportsOptionalMagic()
    {
        var character = CreateTestCharacter();

        Assert.Null(character.Magic);

        character.Magic = new MagicUser();

        Assert.NotNull(character.Magic);
    }

    [Fact]
    public void Character_SupportsOptionalRangedWeapon()
    {
        var character = CreateTestCharacter();

        Assert.Null(character.RangedWeapon);
        Assert.Null(character.RangedSkill);

        character.RangedWeapon = RangedWeapon.LongBow();
        character.RangedSkill = 7;

        Assert.NotNull(character.RangedWeapon);
        Assert.Equal(7, character.RangedSkill);
    }

    [Fact]
    public void Character_InitializesWithZeroExhaustion()
    {
        var character = CreateTestCharacter();

        Assert.Equal(0, character.Exhaustion);
    }
}
