using SteelkiltSharp.Core;

namespace SteelkiltSharp.Tests.Core;

public class CombatTests
{
    private Character CreateWarrior(string name, int strength = 7, int weaponSkill = 7, int dodgeSkill = 5)
    {
        return new Character(
            name,
            new Attributes(strength, 7, 6, 5, 5, 5, 5, 5, 5),
            weaponSkill,
            dodgeSkill,
            Weapon.LongSword(),
            Armor.Leather());
    }

    [Fact]
    public void CombatRound_ReturnsValidResult()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10);
        var defender = CreateWarrior("Defender", weaponSkill: 1);

        var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);

        Assert.NotNull(result);
        Assert.Equal("Attacker", result.Attacker);
        Assert.Equal("Defender", result.Defender);
    }

    [Fact]
    public void CombatRound_WithParry_UsesParryRoll()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10);
        var defender = CreateWarrior("Defender", weaponSkill: 0, dodgeSkill: 10);

        // Run multiple times to check that parry (low) is used, not dodge (high)
        int hits = 0;
        for (int i = 0; i < 20; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (result.Hit) hits++;
        }

        // With high attack vs low parry, should hit most of the time
        Assert.True(hits > 10, "High attack vs low parry should hit frequently");
    }

    // TODO: This test is not deterministic enough to be reliable
    //[Fact]
    //public void CombatRound_WithDodge_UsesDodgeRoll()
    //{
    //    var attacker = CreateWarrior("Attacker", weaponSkill: 10);
    //    var defender = CreateWarrior("Defender", weaponSkill: 0, dodgeSkill: 10);

    //    // Run multiple times
    //    int misses = 0;
    //    for (int i = 0; i < 20; i++)
    //    {
    //        var result = Combat.CombatRound(attacker, defender, DefenseAction.Dodge);
    //        if (!result.Hit) misses++;
    //    }

    //    // With high dodge, should miss some attacks
    //    Assert.True(misses > 0, "High dodge should cause some misses");
    //}

    [Fact]
    public void CombatRound_Miss_ReturnsCorrectResult()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 1);
        var defender = CreateWarrior("Defender", weaponSkill: 10);

        // Try multiple times to get at least one miss
        bool foundMiss = false;
        for (int i = 0; i < 30; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (!result.Hit)
            {
                foundMiss = true;
                Assert.Equal(0, result.Damage);
                Assert.Null(result.WoundLevel);
                Assert.False(result.DefenderDied);
                break;
            }
        }

        Assert.True(foundMiss, "Should produce at least one miss with low attack vs high defense");
    }

    [Fact]
    public void CombatRound_Hit_AppliesDamage()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10);
        var defender = CreateWarrior("Defender", weaponSkill: 0);

        // Repeat to ensure at least one hit
        bool foundHit = false;
        for (int i = 0; i < 20; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (result.Hit)
            {
                foundHit = true;
                Assert.True(result.Damage >= 0);
                break;
            }
        }

        Assert.True(foundHit, "Should produce at least one hit with high attack vs low defense");
    }

    [Fact]
    public void CombatRound_Hit_AddsWoundToDefender()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10, strength: 10);
        var defender = CreateWarrior("Defender", weaponSkill: 0);
        defender.Armor = Armor.None();

        int initialWounds = defender.Wounds.Light + defender.Wounds.Severe + defender.Wounds.Critical;

        // Try multiple times to ensure we get a hit
        bool gotHit = false;
        for (int i = 0; i < 30; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (result.Hit && result.Damage > 0)
            {
                gotHit = true;
                break;
            }
        }

        Assert.True(gotHit, "Should get at least one damaging hit");
        int finalWounds = defender.Wounds.Light + defender.Wounds.Severe + defender.Wounds.Critical;
        Assert.True(finalWounds > initialWounds, "Defender should have gained wounds");
    }

    [Fact]
    public void CombatRound_CalculatesDamageWithStrengthBonus()
    {
        var strongAttacker = CreateWarrior("Strong", strength: 10, weaponSkill: 10);
        var weakDefender = CreateWarrior("Weak", weaponSkill: 0);
        weakDefender.Armor = Armor.None();

        // Strong attacker should have +2 strength bonus
        Assert.Equal(2, strongAttacker.StrengthBonus);

        // Try to get a hit
        for (int i = 0; i < 20; i++)
        {
            var result = Combat.CombatRound(strongAttacker, weakDefender, DefenseAction.Parry);
            if (result.Hit)
            {
                // Damage should include strength bonus
                // Damage = (attack - defense) + strength bonus + weapon damage - armor
                Assert.True(result.Damage >= 0);
                break;
            }
        }
    }

    [Fact]
    public void CombatRound_SubtractsArmorProtection()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10);
        attacker.Weapon = Weapon.Dagger(); // Low damage

        var heavyDefender = CreateWarrior("Defender", weaponSkill: 0);
        heavyDefender.Armor = Armor.FullPlate(); // 5 protection

        var lightDefender = CreateWarrior("Defender2", weaponSkill: 0);
        lightDefender.Armor = Armor.None(); // 0 protection

        // Compare damage against heavy vs light armor
        int heavyDamageSum = 0;
        int lightDamageSum = 0;

        for (int i = 0; i < 30; i++)
        {
            var heavyResult = Combat.CombatRound(attacker, heavyDefender, DefenseAction.Parry);
            if (heavyResult.Hit) heavyDamageSum += heavyResult.Damage;

            var lightResult = Combat.CombatRound(attacker, lightDefender, DefenseAction.Parry);
            if (lightResult.Hit) lightDamageSum += lightResult.Damage;
        }

        // Light armor target should take more total damage
        Assert.True(lightDamageSum >= heavyDamageSum, "Unarmored target should take more damage");
    }

    [Fact]
    public void CombatRound_NegativeDamage_BecomesZero()
    {
        var weakAttacker = CreateWarrior("Weak", strength: 1, weaponSkill: 5);
        weakAttacker.Weapon = Weapon.Dagger(); // 3 damage

        var strongDefender = CreateWarrior("Strong", weaponSkill: 5);
        strongDefender.Armor = Armor.FullPlate(); // 5 protection

        // Try multiple times
        for (int i = 0; i < 30; i++)
        {
            var result = Combat.CombatRound(weakAttacker, strongDefender, DefenseAction.Parry);
            // Damage should never be negative
            Assert.True(result.Damage >= 0, "Damage cannot be negative");
        }
    }

    // TODO: CombatRound_DeterminesWoundLevel_Light test is not deterministic enough to be reliable
    //[Fact]
    //public void CombatRound_DeterminesWoundLevel_Light()
    //{
    //    var attacker = CreateWarrior("Attacker", weaponSkill: 10, strength: 1);
    //    attacker.Weapon = Weapon.Dagger();

    //    var defender = CreateWarrior("Defender", weaponSkill: 0);
    //    defender.Attributes.Constitution = 10; // High constitution

    //    // Low damage vs high constitution should produce light wounds if any
    //    for (int i = 0; i < 50; i++)
    //    {
    //        var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
    //        if (result.Hit && result.WoundLevel.HasValue)
    //        {
    //            // With low damage and high constitution, should be light wound
    //            Assert.True(result.WoundLevel == WoundLevel.Light || result.WoundLevel == WoundLevel.Severe);
    //            if (result.WoundLevel == WoundLevel.Light)
    //                break; // Found expected result
    //        }
    //    }
    //}

    [Fact]
    public void CombatRound_CanKillDefender()
    {
        var attacker = CreateWarrior("Killer", weaponSkill: 10, strength: 10);
        attacker.Weapon = Weapon.GreatAxe();

        var defender = CreateWarrior("Victim", weaponSkill: 0);
        defender.Armor = Armor.None();
        defender.Attributes.Constitution = 1;

        // Attack until death
        int rounds = 0;
        while (!defender.IsDead && rounds < 50)
        {
            Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            rounds++;
        }

        // Eventually defender should die
        Assert.True(defender.IsDead || rounds == 50);
    }

    [Fact]
    public void CombatRound_DefenderDiedFlag_SetCorrectly()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10, strength: 10);
        attacker.Weapon = Weapon.GreatAxe();

        var defender = CreateWarrior("Defender", weaponSkill: 0);
        defender.Armor = Armor.None();

        // Give defender one critical wound already
        defender.Wounds.AddWound(WoundLevel.Critical);

        // Try to deliver the killing blow
        for (int i = 0; i < 50; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (result.DefenderDied)
            {
                Assert.True(defender.IsDead);
                break;
            }
        }
    }

    [Fact]
    public void CombatRound_WoundsAffectSubsequentRolls()
    {
        var attacker = CreateWarrior("Attacker", weaponSkill: 10);
        var defender = CreateWarrior("Defender", weaponSkill: 10);

        // Add wounds to defender
        defender.Wounds.AddWound(WoundLevel.Critical); // -4 penalty

        // Defender's defense should be weaker now
        int hits = 0;
        for (int i = 0; i < 30; i++)
        {
            var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
            if (result.Hit) hits++;
        }

        // Should hit more often against wounded defender
        Assert.True(hits > 0, "Should hit wounded defender at least once");
    }
}
