using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;

namespace SteelkiltSharp.Examples;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== SteelkiltSharp - Draft RPG Combat System ===\n");

        // Example 1: Basic Combat
        Console.WriteLine("--- Example 1: Basic Melee Combat ---");
        BasicCombatExample();
        Console.WriteLine();

        // Example 2: Combat with Wounds
        Console.WriteLine("--- Example 2: Combat Until Victory ---");
        CombatUntilVictoryExample();
        Console.WriteLine();

        // Example 3: Ranged Combat
        Console.WriteLine("--- Example 3: Ranged Combat ---");
        RangedCombatExample();
        Console.WriteLine();

        // Example 4: Magic System
        Console.WriteLine("--- Example 4: Wizard's Duel ---");
        MagicCombatExample();
        Console.WriteLine();

        // Example 5: Advanced Features
        Console.WriteLine("--- Example 5: Advanced Combat Features ---");
        AdvancedFeaturesExample();
        Console.WriteLine();

        Console.WriteLine("=== All Examples Complete ===");
    }

    static void BasicCombatExample()
    {
        // Create two warriors
        var warrior1 = new Character(
            "Sir Roland",
            new Attributes(8, 7, 7, 5, 5, 6, 5, 6, 5),
            weaponSkill: 7,
            dodgeSkill: 5,
            Weapon.LongSword(),
            Armor.Chain());

        var warrior2 = new Character(
            "Blackheart",
            new Attributes(9, 6, 8, 4, 5, 7, 3, 5, 4),
            weaponSkill: 6,
            dodgeSkill: 4,
            Weapon.TwoHandedSword(),
            Armor.Plate());

        Console.WriteLine(warrior1);
        Console.WriteLine(warrior2);
        Console.WriteLine();

        // Execute 3 combat rounds
        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"Round {i}:");
            var result = Combat.CombatRound(warrior1, warrior2, DefenseAction.Parry);
            Console.WriteLine(result);
            Console.WriteLine($"  {warrior2.Name} wounds: {warrior2.Wounds}");
        }
    }

    static void CombatUntilVictoryExample()
    {
        var hero = new Character(
            "Hero",
            new Attributes(7, 8, 6, 6, 5, 6, 7, 6, 6),
            weaponSkill: 6,
            dodgeSkill: 6,
            Weapon.LongSword(),
            Armor.Leather());

        var goblin = new Character(
            "Goblin",
            new Attributes(4, 7, 4, 5, 5, 4, 3, 6, 3),
            weaponSkill: 4,
            dodgeSkill: 5,
            Weapon.Dagger(),
            Armor.None());

        int round = 1;
        while (!hero.IsDead && !goblin.IsDead)
        {
            Console.WriteLine($"Round {round}:");

            // Hero attacks
            var heroResult = Combat.CombatRound(hero, goblin, DefenseAction.Dodge);
            Console.WriteLine($"  {heroResult}");

            if (goblin.IsDead)
            {
                Console.WriteLine($"  {goblin.Name} has been defeated!");
                break;
            }

            // Goblin counterattacks
            var goblinResult = Combat.CombatRound(goblin, hero, DefenseAction.Parry);
            Console.WriteLine($"  {goblinResult}");

            if (hero.IsDead)
            {
                Console.WriteLine($"  {hero.Name} has been defeated!");
                break;
            }

            Console.WriteLine($"  {hero.Name}: {hero.Wounds} | {goblin.Name}: {goblin.Wounds}");
            round++;

            if (round > 20)
            {
                Console.WriteLine("  Combat lasted more than 20 rounds!");
                break;
            }
        }
    }

    static void RangedCombatExample()
    {
        var archer = new Character(
            "Legolas",
            new Attributes(6, 9, 6, 7, 6, 6, 7, 8, 6),
            weaponSkill: 5,
            dodgeSkill: 7,
            Weapon.Dagger(),
            Armor.Leather())
        {
            RangedWeapon = RangedWeapon.LongBow(),
            RangedSkill = 8
        };

        var target = new Character(
            "Orc",
            new Attributes(8, 5, 7, 4, 4, 6, 3, 4, 3),
            weaponSkill: 5,
            dodgeSkill: 3,
            Weapon.GreatAxe(),
            Armor.Chain());

        Console.WriteLine($"{archer.Name} has a {archer.RangedWeapon.Name}");
        Console.WriteLine($"Target: {target.Name}\n");

        // Try different ranges
        int[] distances = { 5, 40, 90, 150 };

        foreach (int distance in distances)
        {
            Console.WriteLine($"Shooting at {distance} feet:");
            var result = RangedCombat.RangedAttack(archer, target, distance, aiming: false);
            Console.WriteLine($"  {result}");
        }

        Console.WriteLine($"\nShooting with aiming bonus at 90 feet:");
        var aimedResult = RangedCombat.RangedAttack(archer, target, 90, aiming: true);
        Console.WriteLine($"  {aimedResult}");
    }

    static void MagicCombatExample()
    {
        var wizard = new Character(
            "Gandalf",
            new Attributes(5, 6, 6, 8, 9, 8, 7, 7, 7),
            weaponSkill: 3,
            dodgeSkill: 5,
            Weapon.Dagger(),
            Armor.None())
        {
            Magic = new MagicUser()
        };

        wizard.Magic.SetBranchSkill(MagicBranch.Elementalism, 8);
        wizard.Magic.SetBranchSkill(MagicBranch.Abjuration, 6);
        wizard.Magic.SetBranchSkill(MagicBranch.Divination, 7);

        // Create spell instances
        var fireball = MagicSystem.Spells.Fireball();
        var magicMissile = MagicSystem.Spells.MagicMissile();
        var shield = MagicSystem.Spells.Shield();

        // Prepare spells
        wizard.Magic.PrepareSpell(fireball);
        wizard.Magic.PrepareSpell(magicMissile);
        wizard.Magic.PrepareSpell(shield);

        Console.WriteLine($"{wizard.Name} - Master of Elementalism");
        Console.WriteLine($"Prepared spells: {string.Join(", ", wizard.Magic.PreparedSpells.Select(s => s.Name))}\n");

        // Cast spells
        Console.WriteLine("Casting Magic Missile:");
        var spell1 = MagicSystem.CastSpell(wizard, magicMissile);
        Console.WriteLine($"  {spell1}");
        Console.WriteLine($"  Exhaustion: {wizard.Exhaustion}\n");

        Console.WriteLine("Casting Fireball:");
        var spell2 = MagicSystem.CastSpell(wizard, fireball);
        Console.WriteLine($"  {spell2}");
        Console.WriteLine($"  Exhaustion: {wizard.Exhaustion}\n");

        Console.WriteLine("Casting Shield:");
        var spell3 = MagicSystem.CastSpell(wizard, shield);
        Console.WriteLine($"  {spell3}");
        Console.WriteLine($"  Total Exhaustion: {wizard.Exhaustion}");
        Console.WriteLine($"  Exhaustion Penalty: {ExhaustionSystem.GetPenalty(wizard.Exhaustion)}");
    }

    static void AdvancedFeaturesExample()
    {
        var fighter = new Character(
            "Conan",
            new Attributes(9, 8, 9, 6, 5, 8, 6, 7, 5),
            weaponSkill: 8,
            dodgeSkill: 6,
            Weapon.TwoHandedSword(),
            Armor.Chain());

        var enemy = new Character(
            "Beast",
            new Attributes(8, 6, 8, 4, 4, 6, 2, 5, 2),
            weaponSkill: 6,
            dodgeSkill: 4,
            Weapon.GreatAxe(),
            Armor.Leather());

        Console.WriteLine("Testing Hit Locations:");
        for (int i = 0; i < 5; i++)
        {
            var location = HitLocation.RollHitLocation();
            Console.WriteLine($"  Hit: {location.Description}");
        }

        Console.WriteLine("\nTesting Combat Maneuvers:");
        foreach (ManeuverType maneuver in Enum.GetValues<ManeuverType>())
        {
            Console.WriteLine($"  {Maneuvers.GetDescription(maneuver)}");
        }

        Console.WriteLine("\nTesting Skill System:");
        var swordSkill = new Skill("Longsword", SkillDifficulty.Average, 5);
        Console.WriteLine($"  {swordSkill}");
        Console.WriteLine($"  Advancement cost: {swordSkill.AdvancementCost()} points");

        Console.WriteLine("\nTesting Exhaustion:");
        fighter.Exhaustion = 0;
        Console.WriteLine($"  Fresh: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
        fighter.Exhaustion = 7;
        Console.WriteLine($"  After 7 points: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
        fighter.Exhaustion = 12;
        Console.WriteLine($"  After 12 points: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
    }
}
