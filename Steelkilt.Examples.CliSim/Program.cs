using SteelkiltSharp.Core;
using SteelkiltSharp.Modules;

namespace Steelkilt.Examples.CliSim
{
    internal class Program
    {
        static void Main(string[] args)
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

            var ManeuverTypeOptions = new Dictionary<char, ManeuverType>
            {
                { 'N', ManeuverType.Normal },
                { 'C', ManeuverType.Charge },
                { 'A', ManeuverType.AllOutAttack },
                { 'D', ManeuverType.DefensivePosition }
            };

            var DefenseActionOptions = new Dictionary<char, DefenseAction>
            {
                { 'P', DefenseAction.Parry },
                { 'D', DefenseAction.Dodge }
            };

            int round = 1;
            while (!hero.IsDead && !goblin.IsDead)
            {
                Console.WriteLine($"Round {round}:");

                var result = ConsolePrompt.GetUserChoice(
                    promptPrefix: $"{hero.Name} stance?",
                    options: ManeuverTypeOptions,
                    defaultValue: ManeuverType.Normal
                );

                /* 
                    Hero attacks
                */

                var heroAttackResult = Combat.CombatRound(hero, goblin, DefenseAction.Dodge);
                Console.WriteLine($"  {heroAttackResult}");

                if (goblin.IsDead)
                {
                    Console.WriteLine($"  {goblin.Name} has been defeated!");
                    break;
                }

                /* 
                    Goblin counterattacks
                */

                // Prompt for defense action
                var defenseActionResult = ConsolePrompt.GetUserChoice(
                    promptPrefix: $"{hero.Name} defense?",
                    options: DefenseActionOptions,
                    defaultValue: DefenseAction.Dodge
                );

                var goblinAttackResult = Combat.CombatRound(goblin, hero, defenseActionResult);
                Console.WriteLine($"  {goblinAttackResult}");

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
    }
}
