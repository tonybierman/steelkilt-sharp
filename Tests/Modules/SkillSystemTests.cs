using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Modules;

public class SkillSystemTests
{
    [Fact]
    public void Skill_Constructor_SetsProperties()
    {
        var skill = new Skill("Swordfighting", SkillDifficulty.Average, 5);

        Assert.Equal("Swordfighting", skill.Name);
        Assert.Equal(SkillDifficulty.Average, skill.Difficulty);
        Assert.Equal(5, skill.Level);
    }

    [Fact]
    public void Skill_Constructor_DefaultsToLevelZero()
    {
        var skill = new Skill("Stealth", SkillDifficulty.Hard);

        Assert.Equal(0, skill.Level);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    [InlineData(15, 10)]
    public void Skill_Constructor_ClampsLevelToValidRange(int input, int expected)
    {
        var skill = new Skill("Test", SkillDifficulty.Easy, input);

        Assert.Equal(expected, skill.Level);
    }

    [Theory]
    [InlineData(SkillDifficulty.Easy, 0, 1)]
    [InlineData(SkillDifficulty.Easy, 5, 6)]
    [InlineData(SkillDifficulty.Average, 0, 2)]
    [InlineData(SkillDifficulty.Average, 5, 12)]
    [InlineData(SkillDifficulty.Hard, 0, 2)]
    [InlineData(SkillDifficulty.Hard, 5, 12)]
    [InlineData(SkillDifficulty.VeryHard, 0, 3)]
    [InlineData(SkillDifficulty.VeryHard, 5, 18)]
    public void Skill_AdvancementCost_CalculatesCorrectly(SkillDifficulty difficulty, int level, int expectedCost)
    {
        var skill = new Skill("Test", difficulty, level);

        Assert.Equal(expectedCost, skill.AdvancementCost());
    }

    [Fact]
    public void Skill_Advance_IncreasesLevel()
    {
        var skill = new Skill("Test", SkillDifficulty.Easy, 5);

        bool result = skill.Advance();

        Assert.True(result);
        Assert.Equal(6, skill.Level);
    }

    [Fact]
    public void Skill_Advance_AtMaxLevel_ReturnsFalse()
    {
        var skill = new Skill("Test", SkillDifficulty.Easy, 10);

        bool result = skill.Advance();

        Assert.False(result);
        Assert.Equal(10, skill.Level);
    }

    [Fact]
    public void Skill_Advance_MultipleTimesFromZero()
    {
        var skill = new Skill("Test", SkillDifficulty.Easy, 0);

        Assert.True(skill.Advance());
        Assert.Equal(1, skill.Level);

        Assert.True(skill.Advance());
        Assert.Equal(2, skill.Level);

        Assert.True(skill.Advance());
        Assert.Equal(3, skill.Level);
    }

    [Fact]
    public void Skill_ToString_ReturnsFormattedString()
    {
        var skill = new Skill("Archery", SkillDifficulty.Hard, 7);

        string result = skill.ToString();

        Assert.Contains("Archery", result);
        Assert.Contains("Hard", result);
        Assert.Contains("7/10", result);
    }

    [Theory]
    [InlineData(SkillDifficulty.Easy, 0, 1)]
    [InlineData(SkillDifficulty.Average, 3, 8)]
    [InlineData(SkillDifficulty.Hard, 5, 12)]
    [InlineData(SkillDifficulty.VeryHard, 9, 30)]
    public void SkillSystem_CalculateAdvancementCost_MatchesSkillMethod(SkillDifficulty difficulty, int level, int expectedCost)
    {
        var skill = new Skill("Test", difficulty, level);
        int skillCost = skill.AdvancementCost();
        int systemCost = SkillSystem.CalculateAdvancementCost(difficulty, level);

        Assert.Equal(expectedCost, skillCost);
        Assert.Equal(expectedCost, systemCost);
        Assert.Equal(skillCost, systemCost);
    }

    [Fact]
    public void SkillSystem_SkillCheck_WithHighSkillAndModifiers_CanSucceed()
    {
        int successes = 0;

        // Roll 30 times with skill 10 vs DC 15 (should succeed sometimes)
        for (int i = 0; i < 30; i++)
        {
            if (SkillSystem.SkillCheck(10, 15, 0))
            {
                successes++;
            }
        }

        Assert.True(successes > 0, "High skill should succeed against moderate DC at least once");
    }

    [Fact]
    public void SkillSystem_SkillCheck_WithLowSkill_CanFail()
    {
        int failures = 0;

        // Roll 30 times with skill 0 vs DC 20 (should fail most/all times)
        for (int i = 0; i < 30; i++)
        {
            if (!SkillSystem.SkillCheck(0, 20, 0))
            {
                failures++;
            }
        }

        Assert.True(failures > 20, "Low skill should fail against high DC most of the time");
    }

    [Fact]
    public void SkillSystem_SkillCheck_WithPositiveModifiers_IncreasesSuccess()
    {
        int successesWithoutBonus = 0;
        int successesWithBonus = 0;

        for (int i = 0; i < 50; i++)
        {
            if (SkillSystem.SkillCheck(5, 15, 0))
                successesWithoutBonus++;

            if (SkillSystem.SkillCheck(5, 15, 5))
                successesWithBonus++;
        }

        Assert.True(successesWithBonus > successesWithoutBonus, "Positive modifiers should increase success rate");
    }

    [Fact]
    public void SkillSystem_SkillCheck_WithNegativeModifiers_DecreasesSuccess()
    {
        int successesWithPenalty = 0;
        int successesWithoutPenalty = 0;

        for (int i = 0; i < 50; i++)
        {
            if (SkillSystem.SkillCheck(8, 15, -5))
                successesWithPenalty++;

            if (SkillSystem.SkillCheck(8, 15, 0))
                successesWithoutPenalty++;
        }

        Assert.True(successesWithPenalty < successesWithoutPenalty, "Negative modifiers should decrease success rate");
    }
}
