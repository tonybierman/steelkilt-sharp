# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SteelkiltSharp is a C# implementation of the Steelkilt RPG combat system based on the Draft 0.4 RPG ruleset by Pitt Murmann. This is a port of the original Rust implementation at https://github.com/tonybierman/steelkilt.

Target Framework: .NET 9.0

## Solution Structure

The solution (`steelkilt-sharp.sln`) contains three projects:

1. **Steelkilt/** - Core library (SteelkiltSharp.csproj)
   - `Core/` - Fundamental combat mechanics (Character, Combat, Weapons, Armor, Wounds, Dice)
   - `Modules/` - Optional advanced systems (Magic, Ranged Combat, Skills, Exhaustion, Hit Locations, Maneuvers)

2. **Examples/** - Demo application (SteelkiltSharp.Examples.csproj)
   - `Program.cs` - Five example scenarios demonstrating all major features

3. **SteelkiltSharp.Tests/** - Unit tests (SteelkiltSharp.Tests.csproj)
   - Uses xUnit framework

## Common Development Commands

### Building
```bash
# Build entire solution
dotnet build steelkilt-sharp.sln

# Build specific project
dotnet build Steelkilt/SteelkiltSharp.csproj
```

### Running Examples
```bash
# From solution root
dotnet run --project Examples/SteelkiltSharp.Examples.csproj

# Or directly
cd Examples
dotnet run
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests in specific project
dotnet test SteelkiltSharp.Tests/SteelkiltSharp.Tests.csproj

# Run with detailed output
dotnet test --verbosity detailed
```

### Cleaning
```bash
# Clean build artifacts
dotnet clean

# Remove bin/obj directories completely
find . -type d \( -name "obj" -o -name "bin" \) -exec rm -rf {} +
```

## Core Architecture

### Combat Flow
The combat system is centered around `Combat.CombatRound()`:
1. Attacker rolls: WeaponSkill + d10 + modifiers (strength bonus, wound penalties, armor penalties, exhaustion)
2. Defender rolls: WeaponSkill (parry) or DodgeSkill (dodge) + d10 + same modifiers
3. If attack > defense: Calculate damage = (attack - defense) + strength bonus + weapon damage - armor protection
4. Determine wound level based on damage vs. defender's Constitution
5. Add wound to defender with automatic stacking (4 Light → 1 Severe, 3 Severe → 1 Critical, 2 Critical → Death)

### Character System
Characters have:
- **9 Attributes** organized into Physical (STR/DEX/CON), Mental (REA/INT/WIL), Interactive (CHA/PER/EMP)
- **Skills**: WeaponSkill (melee attack/parry), DodgeSkill, optional RangedSkill
- **Equipment**: Weapon (damage rating), Armor (protection + movement penalty)
- **Wounds**: Three-tier system with automatic stacking mechanics
- **Optional Systems**: Magic (MagicUser), Ranged weapons, Exhaustion tracking

### Wound Mechanics
The `Wounds` class (Steelkilt/Core/Wounds.cs) implements automatic stacking:
- Damage < CON = Light wound
- Damage ≥ CON = Severe wound
- Damage ≥ CON×2 = Critical wound
- Wounds provide cumulative penalties: Light (-1), Severe (-2), Critical (-4)
- Automatic conversion: 4L→1S, 3S→1C
- 2+ Critical wounds = Death

### Module Systems
All advanced features are in `Modules/` namespace and are optional:

- **MagicSystem**: 9 magic branches, spell levels (Minor to Master), casting rolls vs difficulty, exhaustion costs
- **RangedCombat**: Distance modifiers, aiming bonuses (+2), cover penalties, multiple range bands
- **HitLocation**: Body part targeting (Head 1.5×, Torso 1.0×, Limbs 0.75× damage)
- **Maneuvers**: Charge (+2 attack, -2 defense, +2 damage), All-Out Attack, Defensive Position
- **SkillSystem**: Progressive advancement costs based on difficulty and current level
- **ExhaustionSystem**: Cumulative fatigue penalties from combat and magic (-1 to -4)

## Key Design Patterns

### Immutable Data Objects
Weapons, Armor, and Spells are created via static factory methods:
```csharp
Weapon.LongSword()  // Creates new instance
Armor.Chain()
RangedWeapon.LongBow()
MagicSystem.Spells.Fireball()
```

### Roll Methods
Character has three roll methods that incorporate all modifiers:
- `AttackRoll()` - WeaponSkill + d10 + all penalties
- `ParryRoll()` - WeaponSkill + d10 + all penalties
- `DodgeRoll()` - DodgeSkill + d10 + all penalties

These automatically include wound penalties, armor penalties, and exhaustion.

### Optional Features
Advanced features use nullable properties and can be added to characters:
```csharp
character.Magic = new MagicUser();
character.RangedWeapon = RangedWeapon.LongBow();
character.RangedSkill = 8;
```

## Code Conventions

- **Namespace structure**: `SteelkiltSharp.Core` for base mechanics, `SteelkiltSharp.Modules` for optional systems
- **Null safety**: Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings**: Enabled for common namespaces
- **Documentation**: XML doc comments on public APIs
- **Enums**: Used for discrete choices (DefenseAction, WoundLevel, MagicBranch, SpellLevel, etc.)
- **Static classes**: For stateless systems (Combat, RangedCombat, MagicSystem, ExhaustionSystem, etc.)

## Testing Guidelines

Tests use xUnit and should cover:
- Core combat mechanics (hit/miss, damage calculation, wound stacking)
- Edge cases (death threshold, skill clamping 0-10, negative damage = 0)
- Module interactions (magic exhaustion, ranged distance modifiers, hit location multipliers)
- Character stat calculations (StrengthBonus, wound penalties, exhaustion penalties)

## Reference Documentation

The README.md and QUICK_START.md contain:
- Complete feature documentation
- Code examples for all major systems
- Pre-made weapon/armor/spell lists
- Combat mechanics reference
- Magic branch descriptions

When adding features, update both files to maintain consistency.
