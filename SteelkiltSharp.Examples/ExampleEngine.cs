using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelkiltSharp.Examples
{
    public static class ExampleEngine
    {
        public static void Run(OutputPublisher publisher)
        {
            publisher.Publish("=== SteelkiltSharp - Draft RPG Combat System ===\n");

            // Example 1: Basic Combat
            publisher.Publish("--- Example 1: Basic Melee Combat ---");
            BasicCombatExample(publisher);
            publisher.Publish();

            // Example 2: Combat with Wounds
            publisher.Publish("--- Example 2: Combat Until Victory ---");
            CombatUntilVictoryExample(publisher);
            publisher.Publish();

            // Example 3: Ranged Combat
            publisher.Publish("--- Example 3: Ranged Combat ---");
            RangedCombatExample(publisher);
            publisher.Publish();

            // Example 4: Magic System
            publisher.Publish("--- Example 4: Wizard's Duel ---");
            MagicCombatExample(publisher);
            publisher.Publish();

            // Example 5: Advanced Features
            publisher.Publish("--- Example 5: Advanced Combat Features ---");
            AdvancedFeaturesExample(publisher);
            publisher.Publish();

            publisher.Publish("=== All Examples Complete ===");
        }

        static void BasicCombatExample(OutputPublisher publisher)
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

            publisher.Publish($"{warrior1}");
            publisher.Publish($"{warrior2}");
            publisher.Publish();

            // Execute 3 combat rounds
            for (int i = 1; i <= 3; i++)
            {
                publisher.Publish($"Round {i}:");
                var result = Combat.CombatRound(warrior1, warrior2, DefenseAction.Parry);
                publisher.Publish($"{result}");
                publisher.Publish($"  {warrior2.Name} wounds: {warrior2.Wounds}");
            }
        }

        static void CombatUntilVictoryExample(OutputPublisher publisher)
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
                publisher.Publish($"Round {round}:");

                // Hero attacks
                var heroResult = Combat.CombatRound(hero, goblin, DefenseAction.Dodge);
                publisher.Publish($"  {heroResult}");

                if (goblin.IsDead)
                {
                    publisher.Publish($"  {goblin.Name} has been defeated!");
                    break;
                }

                // Goblin counterattacks
                var goblinResult = Combat.CombatRound(goblin, hero, DefenseAction.Parry);
                publisher.Publish($"  {goblinResult}");

                if (hero.IsDead)
                {
                    publisher.Publish($"  {hero.Name} has been defeated!");
                    break;
                }

                publisher.Publish($"  {hero.Name}: {hero.Wounds} | {goblin.Name}: {goblin.Wounds}");
                round++;

                if (round > 20)
                {
                    publisher.Publish("  Combat lasted more than 20 rounds!");
                    break;
                }
            }
        }

        static void RangedCombatExample(OutputPublisher publisher)
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

            publisher.Publish($"{archer.Name} has a {archer.RangedWeapon.Name}");
            publisher.Publish($"Target: {target.Name}\n");

            // Try different ranges
            int[] distances = { 5, 40, 90, 150 };

            foreach (int distance in distances)
            {
                publisher.Publish($"Shooting at {distance} feet:");
                var result = RangedCombat.RangedAttack(archer, target, distance, aiming: false);
                publisher.Publish($"  {result}");
            }

            publisher.Publish($"\nShooting with aiming bonus at 90 feet:");
            var aimedResult = RangedCombat.RangedAttack(archer, target, 90, aiming: true);
            publisher.Publish($"  {aimedResult}");
        }

        static void MagicCombatExample(OutputPublisher publisher)
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

            publisher.Publish($"{wizard.Name} - Master of Elementalism");
            publisher.Publish($"Prepared spells: {string.Join(", ", wizard.Magic.PreparedSpells.Select(s => s.Name))}\n");

            // Cast spells
            publisher.Publish("Casting Magic Missile:");
            var spell1 = MagicSystem.CastSpell(wizard, magicMissile);
            publisher.Publish($"  {spell1}");
            publisher.Publish($"  Exhaustion: {wizard.Exhaustion}\n");

            publisher.Publish("Casting Fireball:");
            var spell2 = MagicSystem.CastSpell(wizard, fireball);
            publisher.Publish($"  {spell2}");
            publisher.Publish($"  Exhaustion: {wizard.Exhaustion}\n");

            publisher.Publish("Casting Shield:");
            var spell3 = MagicSystem.CastSpell(wizard, shield);
            publisher.Publish($"  {spell3}");
            publisher.Publish($"  Total Exhaustion: {wizard.Exhaustion}");
            publisher.Publish($"  Exhaustion Penalty: {ExhaustionSystem.GetPenalty(wizard.Exhaustion)}");
        }

        static void AdvancedFeaturesExample(OutputPublisher publisher)
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

            publisher.Publish("Testing Hit Locations:");
            for (int i = 0; i < 5; i++)
            {
                var location = HitLocation.RollHitLocation();
                publisher.Publish($"  Hit: {location.Description}");
            }

            publisher.Publish("\nTesting Combat Maneuvers:");
            foreach (ManeuverType maneuver in Enum.GetValues<ManeuverType>())
            {
                publisher.Publish($"  {Maneuvers.GetDescription(maneuver)}");
            }

            publisher.Publish("\nTesting Skill System:");
            var swordSkill = new Skill("Longsword", SkillDifficulty.Average, 5);
            publisher.Publish($"  {swordSkill}");
            publisher.Publish($"  Advancement cost: {swordSkill.AdvancementCost()} points");

            publisher.Publish("\nTesting Exhaustion:");
            fighter.Exhaustion = 0;
            publisher.Publish($"  Fresh: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
            fighter.Exhaustion = 7;
            publisher.Publish($"  After 7 points: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
            fighter.Exhaustion = 12;
            publisher.Publish($"  After 12 points: {ExhaustionSystem.GetExhaustionDescription(fighter.Exhaustion)}");
        }
    }
}
