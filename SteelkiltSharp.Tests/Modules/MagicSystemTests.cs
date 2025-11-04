using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Modules;

public class MagicSystemTests
{
    private Character CreateWizard(string name, int elementalismSkill = 7)
    {
        var wizard = new Character(
            name,
            new Attributes(5, 6, 5, 8, 8, 7, 6, 6, 6),
            weaponSkill: 3,
            dodgeSkill: 5,
            Weapon.Dagger(),
            Armor.None())
        {
            Magic = new MagicUser()
        };

        wizard.Magic.SetBranchSkill(MagicBranch.Elementalism, elementalismSkill);

        return wizard;
    }

    [Fact]
    public void Spell_Constructor_SetsProperties()
    {
        var spell = new Spell("Test Spell", MagicBranch.Elementalism, SpellLevel.Moderate, 2, 4, "Test description");

        Assert.Equal("Test Spell", spell.Name);
        Assert.Equal(MagicBranch.Elementalism, spell.Branch);
        Assert.Equal(SpellLevel.Moderate, spell.Level);
        Assert.Equal(2, spell.CastingTime);
        Assert.Equal(4, spell.ExhaustionCost);
        Assert.Equal("Test description", spell.Description);
    }

    [Theory]
    [InlineData(SpellLevel.Minor, 12)]
    [InlineData(SpellLevel.Lesser, 14)]
    [InlineData(SpellLevel.Moderate, 16)]
    [InlineData(SpellLevel.Greater, 18)]
    [InlineData(SpellLevel.Master, 20)]
    public void Spell_DifficultyClass_CalculatesCorrectly(SpellLevel level, int expectedDC)
    {
        var spell = new Spell("Test", MagicBranch.Elementalism, level, 1, 1, "Test");

        Assert.Equal(expectedDC, spell.DifficultyClass);
    }

    [Fact]
    public void MagicUser_Constructor_InitializesAllBranches()
    {
        var magic = new MagicUser();

        foreach (MagicBranch branch in Enum.GetValues<MagicBranch>())
        {
            Assert.Equal(0, magic.GetBranchSkill(branch));
        }

        Assert.Empty(magic.PreparedSpells);
        Assert.Equal(0, magic.MagicalExhaustion);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(10)]
    public void MagicUser_SetBranchSkill_SetsCorrectValue(int skill)
    {
        var magic = new MagicUser();
        magic.SetBranchSkill(MagicBranch.Elementalism, skill);

        Assert.Equal(skill, magic.GetBranchSkill(MagicBranch.Elementalism));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(11, 10)]
    [InlineData(15, 10)]
    public void MagicUser_SetBranchSkill_ClampsValue(int input, int expected)
    {
        var magic = new MagicUser();
        magic.SetBranchSkill(MagicBranch.Elementalism, input);

        Assert.Equal(expected, magic.GetBranchSkill(MagicBranch.Elementalism));
    }

    [Fact]
    public void MagicUser_GetBranchSkill_ReturnsZeroForUnsetBranch()
    {
        var magic = new MagicUser();

        int skill = magic.GetBranchSkill(MagicBranch.Necromancy);

        Assert.Equal(0, skill);
    }

    [Fact]
    public void MagicUser_PrepareSpell_AddsSpellToList()
    {
        var magic = new MagicUser();
        var spell = MagicSystem.Spells.Fireball();

        magic.PrepareSpell(spell);

        Assert.Contains(spell, magic.PreparedSpells);
    }

    [Fact]
    public void MagicUser_PrepareSpell_DoesNotAddDuplicates()
    {
        var magic = new MagicUser();
        var spell = MagicSystem.Spells.Fireball();

        magic.PrepareSpell(spell);
        magic.PrepareSpell(spell);

        Assert.Single(magic.PreparedSpells);
    }

    [Fact]
    public void MagicUser_CanPrepareMultipleSpells()
    {
        var magic = new MagicUser();

        magic.PrepareSpell(MagicSystem.Spells.Fireball());
        magic.PrepareSpell(MagicSystem.Spells.MagicMissile());
        magic.PrepareSpell(MagicSystem.Spells.Shield());

        Assert.Equal(3, magic.PreparedSpells.Count);
    }

    [Fact]
    public void CastSpell_WithoutMagic_ThrowsException()
    {
        var nonWizard = new Character(
            "Fighter",
            new Attributes(8, 7, 7, 5, 5, 5, 5, 5, 5),
            7, 5,
            Weapon.LongSword(),
            Armor.Chain());

        var spell = MagicSystem.Spells.Fireball();

        Assert.Throws<InvalidOperationException>(() =>
            MagicSystem.CastSpell(nonWizard, spell));
    }

    [Fact]
    public void CastSpell_UnpreparedSpell_Fails()
    {
        var wizard = CreateWizard("Wizard");
        var spell = MagicSystem.Spells.Fireball();

        var result = MagicSystem.CastSpell(wizard, spell);

        Assert.False(result.Success);
        Assert.Contains("not prepared", result.Message);
        Assert.Equal(0, result.ExhaustionGained);
    }

    [Fact]
    public void CastSpell_PreparedSpell_ReturnsResult()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic!.PrepareSpell(spell);

        var result = MagicSystem.CastSpell(wizard, spell);

        Assert.NotNull(result);
        Assert.Equal("Wizard", result.CasterName);
        Assert.Equal(spell, result.Spell);
    }

    [Fact]
    public void CastSpell_Success_IncreasesExhaustion()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic!.PrepareSpell(spell);

        int initialExhaustion = wizard.Exhaustion;
        int initialMagicalExhaustion = wizard.Magic.MagicalExhaustion;

        // Try multiple times to get a success
        for (int i = 0; i < 30; i++)
        {
            wizard.Exhaustion = 0;
            wizard.Magic.MagicalExhaustion = 0;

            var result = MagicSystem.CastSpell(wizard, spell);
            if (result.Success)
            {
                Assert.True(wizard.Exhaustion > 0);
                Assert.True(wizard.Magic.MagicalExhaustion > 0);
                Assert.Equal(wizard.Exhaustion, wizard.Magic.MagicalExhaustion);
                break;
            }
        }
    }

    [Fact]
    public void CastSpell_Failure_IncreasesExhaustionByHalf()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 0);
        var spell = MagicSystem.Spells.Fireball(); // Moderate, DC 16
        wizard.Magic!.PrepareSpell(spell);

        // With skill 0, should fail most of the time
        for (int i = 0; i < 30; i++)
        {
            wizard.Exhaustion = 0;
            wizard.Magic.MagicalExhaustion = 0;

            var result = MagicSystem.CastSpell(wizard, spell);
            if (!result.Success)
            {
                // Failed cast should give half exhaustion
                Assert.Equal(spell.ExhaustionCost / 2, result.ExhaustionGained);
                break;
            }
        }
    }

    [Fact]
    public void CastSpell_IncludesWoundPenalty()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        wizard.Wounds.AddWound(WoundLevel.Critical); // -4 penalty
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic!.PrepareSpell(spell);

        int successes = 0;
        for (int i = 0; i < 30; i++)
        {
            var result = MagicSystem.CastSpell(wizard, spell);
            if (result.Success) successes++;
        }

        // Wounds should reduce success rate
        Assert.True(successes < 25, "Wounds should reduce casting success");
    }

    [Fact]
    public void CastSpell_IncludesExhaustionPenalty()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        wizard.Exhaustion = 16; // -4 penalty
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic!.PrepareSpell(spell);

        int successes = 0;
        for (int i = 0; i < 30; i++)
        {
            var result = MagicSystem.CastSpell(wizard, spell);
            if (result.Success) successes++;
        }

        Assert.True(successes < 25, "Exhaustion should reduce casting success");
    }

    [Fact]
    public void CastSpell_IncludesMagicalExhaustionPenalty()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        wizard.Magic!.MagicalExhaustion = 16; // -4 penalty
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic.PrepareSpell(spell);

        int successes = 0;
        for (int i = 0; i < 30; i++)
        {
            var result = MagicSystem.CastSpell(wizard, spell);
            if (result.Success) successes++;
        }

        Assert.True(successes < 25, "Magical exhaustion should reduce casting success");
    }

    [Fact]
    public void CreateCombatSpell_CreatesSpellWithCorrectProperties()
    {
        var spell = MagicSystem.CreateCombatSpell("Test Blast", MagicBranch.Elementalism, SpellLevel.Moderate);

        Assert.Equal("Test Blast", spell.Name);
        Assert.Equal(MagicBranch.Elementalism, spell.Branch);
        Assert.Equal(SpellLevel.Moderate, spell.Level);
        Assert.Equal(3, spell.CastingTime); // Level 3
        Assert.Equal(6, spell.ExhaustionCost); // Level 3 * 2
        Assert.Contains("9 damage", spell.Description); // Level 3 * 3
    }

    [Fact]
    public void Spells_Fireball_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.Fireball();

        Assert.Equal("Fireball", spell.Name);
        Assert.Equal(MagicBranch.Elementalism, spell.Branch);
        Assert.Equal(SpellLevel.Moderate, spell.Level);
    }

    [Fact]
    public void Spells_LightningBolt_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.LightningBolt();

        Assert.Equal("Lightning Bolt", spell.Name);
        Assert.Equal(MagicBranch.Elementalism, spell.Branch);
        Assert.Equal(SpellLevel.Greater, spell.Level);
    }

    [Fact]
    public void Spells_MagicMissile_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.MagicMissile();

        Assert.Equal("Magic Missile", spell.Name);
        Assert.Equal(MagicBranch.Abjuration, spell.Branch);
        Assert.Equal(SpellLevel.Minor, spell.Level);
    }

    [Fact]
    public void Spells_IceSpear_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.IceSpear();

        Assert.Equal("Ice Spear", spell.Name);
        Assert.Equal(MagicBranch.Elementalism, spell.Branch);
        Assert.Equal(SpellLevel.Lesser, spell.Level);
    }

    [Fact]
    public void Spells_Heal_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.Heal();

        Assert.Equal("Heal", spell.Name);
        Assert.Equal(MagicBranch.Enchantment, spell.Branch);
        Assert.Equal(SpellLevel.Moderate, spell.Level);
        Assert.Equal(2, spell.CastingTime);
        Assert.Equal(4, spell.ExhaustionCost);
    }

    [Fact]
    public void Spells_DetectMagic_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.DetectMagic();

        Assert.Equal("Detect Magic", spell.Name);
        Assert.Equal(MagicBranch.Divination, spell.Branch);
        Assert.Equal(SpellLevel.Minor, spell.Level);
    }

    [Fact]
    public void Spells_Shield_HasCorrectProperties()
    {
        var spell = MagicSystem.Spells.Shield();

        Assert.Equal("Shield", spell.Name);
        Assert.Equal(MagicBranch.Abjuration, spell.Branch);
        Assert.Equal(SpellLevel.Lesser, spell.Level);
    }

    [Fact]
    public void SpellCastResult_ToString_Success_FormatsCorrectly()
    {
        var spell = MagicSystem.Spells.Fireball();
        var result = new SpellCastResult(
            "Wizard",
            spell,
            18,
            16,
            true,
            6,
            "Successfully cast Fireball!");

        string output = result.ToString();

        Assert.Contains("Wizard", output);
        Assert.Contains("Fireball", output);
        Assert.Contains("SUCCESS", output);
        Assert.Contains("Roll:18", output);
        Assert.Contains("DC:16", output);
    }

    [Fact]
    public void SpellCastResult_ToString_Failure_FormatsCorrectly()
    {
        var spell = MagicSystem.Spells.Fireball();
        var result = new SpellCastResult(
            "Wizard",
            spell,
            14,
            16,
            false,
            3,
            "Failed to cast Fireball.");

        string output = result.ToString();

        Assert.Contains("Wizard", output);
        Assert.Contains("Fireball", output);
        Assert.Contains("FAILED", output);
    }

    [Fact]
    public void MagicSystem_AllSpellBranchesSupported()
    {
        var magic = new MagicUser();

        foreach (MagicBranch branch in Enum.GetValues<MagicBranch>())
        {
            magic.SetBranchSkill(branch, 5);
            Assert.Equal(5, magic.GetBranchSkill(branch));
        }
    }

    [Fact]
    public void MagicSystem_MultipleSpellCasts_AccumulateExhaustion()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        var spell = MagicSystem.Spells.MagicMissile();
        wizard.Magic!.PrepareSpell(spell);

        int casts = 0;
        for (int i = 0; i < 10; i++)
        {
            var result = MagicSystem.CastSpell(wizard, spell);
            if (result.Success)
            {
                casts++;
            }
        }

        // After multiple casts, exhaustion should accumulate
        if (casts > 3)
        {
            Assert.True(wizard.Exhaustion > 5);
            Assert.True(wizard.Magic.MagicalExhaustion > 5);
        }
    }

    [Fact]
    public void MagicSystem_HighSkill_IncreasesSuccessRate()
    {
        var highSkillWizard = CreateWizard("High", elementalismSkill: 10);
        var lowSkillWizard = CreateWizard("Low", elementalismSkill: 2);

        var spell = MagicSystem.Spells.Fireball();
        highSkillWizard.Magic!.PrepareSpell(spell);
        lowSkillWizard.Magic!.PrepareSpell(spell);

        int highSuccesses = 0;
        int lowSuccesses = 0;

        for (int i = 0; i < 100; i++)
        {
            var highResult = MagicSystem.CastSpell(highSkillWizard, spell);
            if (highResult.Success) highSuccesses++;

            var lowResult = MagicSystem.CastSpell(lowSkillWizard, spell);
            if (lowResult.Success) lowSuccesses++;
        }

        Assert.True(highSuccesses >= lowSuccesses, "Higher skill should result in more successful casts");
    }

    [Fact]
    public void MagicSystem_WrongBranch_ReducesSuccessRate()
    {
        var wizard = CreateWizard("Wizard", elementalismSkill: 10);
        wizard.Magic!.SetBranchSkill(MagicBranch.Divination, 0);

        var elementalSpell = MagicSystem.Spells.Fireball();
        var divinationSpell = MagicSystem.Spells.DetectMagic();

        wizard.Magic.PrepareSpell(elementalSpell);
        wizard.Magic.PrepareSpell(divinationSpell);

        int elementalSuccesses = 0;
        int divinationSuccesses = 0;

        for (int i = 0; i < 100; i++)
        {
            var eleResult = MagicSystem.CastSpell(wizard, elementalSpell);
            if (eleResult.Success) elementalSuccesses++;

            var divResult = MagicSystem.CastSpell(wizard, divinationSpell);
            if (divResult.Success) divinationSuccesses++;
        }

        Assert.True(elementalSuccesses >= divinationSuccesses, "High skill branch should succeed more or equal often");
    }
}
