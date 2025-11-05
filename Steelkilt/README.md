# Steelkilt Sharp

[Steelkilt Sharp](https://github.com/tonybierman/steelkilt-sharp) is a C# implementation of the [Steelkilt](https://github.com/tonybierman/steelkilt) RPG combat system based on the Draft 0.4 RPG ruleset by Pitt Murmann.

## Overview

Steelkilt Sharp is a combat system library for role-playing games, providing mechanics for character creation, combat resolution, skill progression, and advanced features like magic and ranged combat.

## Features

### Core Systems

- **Nine-Attribute Character System**: Characters have 9 attributes organized into Physical (Strength, Dexterity, Constitution), Mental (Reason, Intuition, Willpower), and Interactive (Charisma, Perception, Empathy) categories
- **D10-Based Combat**: Attack and defense rolls use d10 dice combined with skills and modifiers
- **Wound Tracking**: Three wound levels (Light, Severe, Critical) with automatic stacking mechanics:
  - 4 Light wounds → 1 Severe wound
  - 3 Severe wounds → 1 Critical wound
  - 2 Critical wounds → Death
- **Equipment System**: Weapons with impact ratings and armor with protection values

### Advanced Modules

- **Skill System**: Progressive skill development with difficulty-based advancement costs
- **Exhaustion Tracking**: Combat fatigue with cumulative penalties (-1 to -4)
- **Combat Maneuvers**: Tactical options including Charge, All-Out Attack, and Defensive Position
- **Hit Location System**: Body-part targeting with damage multipliers (head 1.5x, limbs 0.75x)
- **Ranged Combat**: Full implementation with distance modifiers, aiming bonuses, and cover penalties
- **Magic System**: Nine magical branches with spell preparation, casting rolls, and magical exhaustion

## Getting Started

## Usage

### Creating Characters

```csharp
using SteelkiltSharp.Core;

var warrior = new Character(
    "Sir Roland",
    new Attributes(8, 7, 7, 5, 5, 6, 5, 6, 5),
    weaponSkill: 7,
    dodgeSkill: 5,
    Weapon.LongSword(),
    Armor.Chain());
```

### Basic Combat

```csharp
using SteelkiltSharp.Core;

var result = Combat.CombatRound(attacker, defender, DefenseAction.Parry);
Console.WriteLine(result);
```

### Ranged Combat

```csharp
using SteelkiltSharp.Modules;

var archer = new Character(/* ... */)
{
    RangedWeapon = RangedWeapon.LongBow(),
    RangedSkill = 8
};

var result = RangedCombat.RangedAttack(archer, target, distance: 50);
```

### Magic System

```csharp
using SteelkiltSharp.Modules;

var wizard = new Character(/* ... */)
{
    Magic = new MagicUser()
};

wizard.Magic.SetBranchSkill(MagicBranch.Elementalism, 8);

var fireball = MagicSystem.Spells.Fireball();
wizard.Magic.PrepareSpell(fireball);

var result = MagicSystem.CastSpell(wizard, fireball);
```

## Combat Mechanics

### Attack Resolution

1. Attacker rolls: Weapon Skill + d10 + Modifiers
2. Defender rolls: Weapon Skill (parry) or Dodge Skill + d10 + Modifiers
3. If attack > defense: Hit!
4. Damage = (Attack Roll - Defense Roll) + Strength Bonus + Weapon Damage - Armor Protection
5. Wound severity determined by damage vs. defender's Constitution

### Wound Levels

- **Light Wound**: Damage < Constitution
- **Severe Wound**: Damage ≥ Constitution
- **Critical Wound**: Damage ≥ Constitution × 2

### Strength Bonus

- STR ≤ 2: -1 damage
- STR ≥ 7: +1 damage
- STR ≥ 9: +2 damage

## Magic Branches

1. **Divination**: Knowledge and foresight
2. **Alchemy**: Material transformation
3. **Elementalism**: Fire, ice, lightning
4. **Enchantment**: Enhancement and buff magic
5. **Illusion**: Deception and trickery
6. **Necromancy**: Death and undeath
7. **Summoning**: Creature summoning
8. **Transmutation**: Shape and form alteration
9. **Abjuration**: Protection and barriers

## License

This project is released under the MIT License. The original Draft 0.4 RPG Rule Set was published under the OpenContent License Version 1.0.

## Credits

- Original Steelkilt Rust implementation on GitHub: [tonybierman/steelkilt](https://github.com/tonybierman/steelkilt)
- Steelkilt Sharp on GitHib: [tonybierman/steelkilt](https://github.com/tonybierman/steelkilt-sharp)
- Draft 0.4 RPG System: Pitt Murmann

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.
