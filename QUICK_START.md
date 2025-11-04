# SteelkiltSharp Quick Start Guide

## Installation

```bash
git clone [your-repo-url]
cd steelkilt_sharp
```

## Basic Usage

### 1. Create a Character

```csharp
using SteelkiltSharp.Core;

var warrior = new Character(
    name: "Conan",
    attributes: new Attributes(
        strength: 9,      // Physical
        dexterity: 8,     // Physical
        constitution: 9,  // Physical
        reason: 6,        // Mental
        intuition: 5,     // Mental
        willpower: 8,     // Mental
        charisma: 6,      // Interactive
        perception: 7,    // Interactive
        empathy: 5        // Interactive
    ),
    weaponSkill: 8,
    dodgeSkill: 6,
    weapon: Weapon.TwoHandedSword(),
    armor: Armor.Chain()
);
```

### 2. Simple Combat

```csharp
using SteelkiltSharp.Core;

// Create two fighters
var fighter1 = new Character(...);
var fighter2 = new Character(...);

// Execute one round of combat
var result = Combat.CombatRound(fighter1, fighter2, DefenseAction.Parry);

// Display result
Console.WriteLine(result);
// Output: "Conan attacks Goblin: HIT for 8 damage - Light wound"

// Check if defender died
if (result.DefenderDied)
{
    Console.WriteLine($"{fighter2.Name} has been defeated!");
}
```

### 3. Complete Combat Loop

```csharp
int round = 1;
while (!hero.IsDead && !monster.IsDead)
{
    Console.WriteLine($"Round {round}:");

    // Hero attacks
    var heroAttack = Combat.CombatRound(hero, monster, DefenseAction.Dodge);
    Console.WriteLine(heroAttack);

    if (monster.IsDead) break;

    // Monster counterattacks
    var monsterAttack = Combat.CombatRound(monster, hero, DefenseAction.Parry);
    Console.WriteLine(monsterAttack);

    round++;
}
```

### 4. Ranged Combat

```csharp
using SteelkiltSharp.Modules;

var archer = new Character(...)
{
    RangedWeapon = RangedWeapon.LongBow(),
    RangedSkill = 7
};

// Shoot at 100 feet
var shot = RangedCombat.RangedAttack(
    attacker: archer,
    defender: target,
    distance: 100,
    aiming: true  // +2 bonus
);

Console.WriteLine(shot);
```

### 5. Magic System

```csharp
using SteelkiltSharp.Modules;

// Create a wizard
var wizard = new Character(...)
{
    Magic = new MagicUser()
};

// Set magic skills
wizard.Magic.SetBranchSkill(MagicBranch.Elementalism, 7);

// Prepare spells
var fireball = MagicSystem.Spells.Fireball();
wizard.Magic.PrepareSpell(fireball);

// Cast spell
var result = MagicSystem.CastSpell(wizard, fireball);
Console.WriteLine(result);
// Output: "Gandalf casts Fireball: SUCCESS (Roll:15 vs DC:16)"
```

## Pre-made Weapons

```csharp
Weapon.Dagger()         // Small impact (damage 3)
Weapon.LongSword()      // Medium impact (damage 5)
Weapon.TwoHandedSword() // Large impact (damage 7)
Weapon.GreatAxe()       // Huge impact (damage 9)
```

## Pre-made Armor

```csharp
Armor.None()        // 0 protection, 0 penalty
Armor.HeavyCloth()  // 1 protection, 0 penalty
Armor.Leather()     // 2 protection, 0 penalty
Armor.Chain()       // 3 protection, -1 penalty
Armor.Plate()       // 4 protection, -2 penalty
Armor.FullPlate()   // 5 protection, -3 penalty
```

## Pre-made Ranged Weapons

```csharp
RangedWeapon.ShortBow()      // Damage 4, ranges: 30/60/150
RangedWeapon.LongBow()       // Damage 6, ranges: 50/100/250
RangedWeapon.LightCrossbow() // Damage 5, ranges: 40/80/200
RangedWeapon.HeavyCrossbow() // Damage 8, ranges: 50/100/250
```

## Pre-made Spells

```csharp
MagicSystem.Spells.MagicMissile()  // Minor Abjuration
MagicSystem.Spells.IceSpear()      // Lesser Elementalism
MagicSystem.Spells.Fireball()      // Moderate Elementalism
MagicSystem.Spells.LightningBolt() // Greater Elementalism
MagicSystem.Spells.Heal()          // Moderate Enchantment
MagicSystem.Spells.DetectMagic()   // Minor Divination
MagicSystem.Spells.Shield()        // Lesser Abjuration
```

## Advanced Features

### Combat Maneuvers

```csharp
using SteelkiltSharp.Modules;

var mods = Maneuvers.GetModifiers(ManeuverType.Charge);
// mods.AttackBonus = +2
// mods.DefenseBonus = -2
// mods.DamageBonus = +2
```

### Hit Locations

```csharp
using SteelkiltSharp.Modules;

var location = HitLocation.RollHitLocation();
// Returns: Head (1.5x), Torso (1.0x), or Limbs (0.75x)

int finalDamage = HitLocation.ApplyLocationDamage(baseDamage, BodyLocation.Head);
```

### Skills

```csharp
using SteelkiltSharp.Modules;

var skill = new Skill("Swordfighting", SkillDifficulty.Average, level: 5);
int cost = skill.AdvancementCost(); // 12 points
skill.Advance(); // Now level 6
```

### Exhaustion

```csharp
using SteelkiltSharp.Modules;

character.Exhaustion = 8;
int penalty = ExhaustionSystem.GetPenalty(character.Exhaustion); // -2
string desc = ExhaustionSystem.GetExhaustionDescription(character.Exhaustion);
// "Tired (-2)"
```

## Common Patterns

### Check Character Status

```csharp
Console.WriteLine($"Wounds: {character.Wounds}");
Console.WriteLine($"Dead: {character.IsDead}");
Console.WriteLine($"Exhaustion: {character.Exhaustion}");
Console.WriteLine($"Strength Bonus: {character.StrengthBonus}");
```

### Manual Rolls

```csharp
int attackRoll = character.AttackRoll();  // Weapon skill + d10 + mods
int parryRoll = character.ParryRoll();    // Weapon skill + d10 + mods
int dodgeRoll = character.DodgeRoll();    // Dodge skill + d10 + mods
```

## Next Steps

- See `Examples/Program.cs` for complete working examples
- Read `README.md` for full documentation

## Support

For issues or questions, see the project repository.
