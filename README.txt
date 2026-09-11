Runebound Depths is a 2D action roguelike developed in Unity and C#.

This repository contains selected source code from the project for
portfolio and code-review purposes. Commercial art, audio, third-party
packages, and the complete Unity project are not included.

## Play the Game

[Download Runebound Depths on itch.io](https://ryder52153.itch.io/runebound-depths)

## Technical Highlights

- Player and enemy state-machine architecture
- Enemy and boss behavior systems
- Character stats, equipment, skills, and item effects
- Reusable object pooling
- Event-driven gameplay systems
- UI/HUD and progression systems

## Featured Code

### State Machine(Source/State_Machine)
Reusable state architecture supporting player movement, combat, abilities,
enemy behavior, boss states, status effects, and death states.

### Object Pooling
Reusable pooling system built around Unity's ObjectPool<T> for enemies,
projectiles, pickups, VFX, and other frequently spawned objects.

### Skill System
Extensible skill framework supporting cooldowns, upgrades, damage scaling,
events, and ScriptableObject-driven configuration.

### Stat System
Character-stat architecture supporting base values, modifiers, modifier
sources, and recalculation of derived values.


## My Role

**Solo Developer / Gameplay Programmer**

I designed and implemented the gameplay systems represented in this
repository.

## Built With

- Unity
- C#
- Git / GitHub

## About This Repository

This is a curated portfolio repository rather than the complete production
project. Third-party packages and licensed game assets are intentionally
excluded.
