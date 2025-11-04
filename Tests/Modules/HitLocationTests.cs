using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Tests.Modules;

public class HitLocationTests
{
    [Fact]
    public void HitLocationResult_Constructor_SetsProperties()
    {
        var result = new HitLocationResult(BodyLocation.Head, 1.5, "Head (1.5x damage)");

        Assert.Equal(BodyLocation.Head, result.Location);
        Assert.Equal(1.5, result.DamageMultiplier);
        Assert.Equal("Head (1.5x damage)", result.Description);
    }

    [Fact]
    public void RollHitLocation_ReturnsValidLocation()
    {
        for (int i = 0; i < 50; i++)
        {
            var result = HitLocation.RollHitLocation();

            Assert.NotNull(result);
            Assert.True(result.DamageMultiplier > 0);
            Assert.NotNull(result.Description);
            Assert.NotEmpty(result.Description);
        }
    }

    [Fact]
    public void RollHitLocation_ProducesVariedResults()
    {
        var locations = new HashSet<BodyLocation>();

        for (int i = 0; i < 100; i++)
        {
            var result = HitLocation.RollHitLocation();
            locations.Add(result.Location);
        }

        // Should get at least 3 different locations in 100 rolls
        Assert.True(locations.Count >= 3, "Should produce varied hit locations");
    }

    [Fact]
    public void RollHitLocation_TorsoIsMostCommon()
    {
        var locationCounts = new Dictionary<BodyLocation, int>();

        for (int i = 0; i < 200; i++)
        {
            var result = HitLocation.RollHitLocation();
            if (!locationCounts.ContainsKey(result.Location))
                locationCounts[result.Location] = 0;
            locationCounts[result.Location]++;
        }

        // Torso should be hit (rolls 3-6, 40% chance)
        Assert.True(locationCounts.ContainsKey(BodyLocation.Torso));
        Assert.True(locationCounts[BodyLocation.Torso] > 50, "Torso should be hit frequently");
    }

    [Fact]
    public void GetLocation_Head_Returns1Point5Multiplier()
    {
        var result = HitLocation.GetLocation(BodyLocation.Head);

        Assert.Equal(BodyLocation.Head, result.Location);
        Assert.Equal(1.5, result.DamageMultiplier);
        Assert.Contains("Head", result.Description);
        Assert.Contains("1.5x", result.Description);
    }

    [Fact]
    public void GetLocation_Torso_Returns1Point0Multiplier()
    {
        var result = HitLocation.GetLocation(BodyLocation.Torso);

        Assert.Equal(BodyLocation.Torso, result.Location);
        Assert.Equal(1.0, result.DamageMultiplier);
        Assert.Contains("Torso", result.Description);
    }

    [Theory]
    [InlineData(BodyLocation.RightArm)]
    [InlineData(BodyLocation.LeftArm)]
    [InlineData(BodyLocation.RightLeg)]
    [InlineData(BodyLocation.LeftLeg)]
    public void GetLocation_Limbs_Returns0Point75Multiplier(BodyLocation limb)
    {
        var result = HitLocation.GetLocation(limb);

        Assert.Equal(limb, result.Location);
        Assert.Equal(0.75, result.DamageMultiplier);
        Assert.Contains("0.75x", result.Description);
    }

    [Theory]
    [InlineData(10, BodyLocation.Head, 15)]
    [InlineData(10, BodyLocation.Torso, 10)]
    [InlineData(10, BodyLocation.RightArm, 8)]
    [InlineData(10, BodyLocation.LeftArm, 8)]
    [InlineData(10, BodyLocation.RightLeg, 8)]
    [InlineData(10, BodyLocation.LeftLeg, 8)]
    [InlineData(8, BodyLocation.Head, 12)]
    [InlineData(9, BodyLocation.RightArm, 7)]
    public void ApplyLocationDamage_CalculatesCorrectly(int baseDamage, BodyLocation location, int expectedDamage)
    {
        int result = HitLocation.ApplyLocationDamage(baseDamage, location);

        Assert.Equal(expectedDamage, result);
    }

    [Fact]
    public void ApplyLocationDamage_RoundsCorrectly()
    {
        // 7 * 0.75 = 5.25, should round to 5
        int result = HitLocation.ApplyLocationDamage(7, BodyLocation.RightArm);
        Assert.Equal(5, result);

        // 9 * 0.75 = 6.75, should round to 7
        result = HitLocation.ApplyLocationDamage(9, BodyLocation.RightArm);
        Assert.Equal(7, result);
    }

    [Fact]
    public void ApplyLocationDamage_ZeroDamage_RemainsZero()
    {
        foreach (BodyLocation location in Enum.GetValues<BodyLocation>())
        {
            int result = HitLocation.ApplyLocationDamage(0, location);
            Assert.Equal(0, result);
        }
    }

    [Fact]
    public void ApplyLocationDamage_HeadIncreasesDamage()
    {
        int baseDamage = 10;
        int headDamage = HitLocation.ApplyLocationDamage(baseDamage, BodyLocation.Head);
        int torsoDamage = HitLocation.ApplyLocationDamage(baseDamage, BodyLocation.Torso);

        Assert.True(headDamage > torsoDamage, "Head hits should do more damage");
    }

    [Fact]
    public void ApplyLocationDamage_LimbsReduceDamage()
    {
        int baseDamage = 10;
        int limbDamage = HitLocation.ApplyLocationDamage(baseDamage, BodyLocation.RightArm);
        int torsoDamage = HitLocation.ApplyLocationDamage(baseDamage, BodyLocation.Torso);

        Assert.True(limbDamage < torsoDamage, "Limb hits should do less damage");
    }

    [Fact]
    public void HitLocation_AllBodyLocationsSupported()
    {
        foreach (BodyLocation location in Enum.GetValues<BodyLocation>())
        {
            var result = HitLocation.GetLocation(location);
            Assert.NotNull(result);
            Assert.True(result.DamageMultiplier > 0);
        }
    }
}
