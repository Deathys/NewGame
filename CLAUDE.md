# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2D roguelite prototype inspired by Dead Cells, targeting desktop (Windows, macOS, Linux) and mobile (iOS, Android) platforms. Built with Unity 2022.3 LTS and Universal Render Pipeline (URP).

## Unity Project Structure

All Unity project files are located in `unity_roguelite_prototype/`. The main working directory is `unity_roguelite_prototype/Assets/Scripts/`.

### Core Script Organization

- **Player/** - Player movement, combat, and health systems
- **Enemies/** - Enemy AI base classes and implementations
- **Systems/** - Game managers, currency, level generation, camera, hit stop
- **UI/** - Currency display, health bars, and UI components
- **Input/** - Input abstraction layer supporting both legacy and new Input System

## Required Unity Packages

- Universal RP (com.unity.render-pipelines.universal)
- 2D Tilemap Extras (com.unity.2d.tilemap.extras)
- Input System (com.unity.inputsystem)
- Cinemachine (com.unity.cinemachine)
- TextMeshPro (com.unity.textmeshpro)

## Key Architecture Patterns

### Package Fallback System

The codebase uses conditional compilation to gracefully handle missing packages:

- `PackageDefines.cs` automatically detects available packages and sets defines
- Three UI variants provided: `CurrencyUI.cs` (full), `SimpleCurrencyDisplay.cs` (basic), `SafeCurrencyUI.cs` (minimal)
- Input system falls back to legacy input when new Input System is unavailable
- Reference pattern in `PlayerController.cs:37-55` and `InputManager.cs`

### Enemy AI State Machine

All enemies inherit from `EnemyStateMachine` which extends `EnemyBase`:

- **States**: Patrol, Aggro, Attack, Cooldown, Death
- Base class handles state transitions at `EnemyStateMachine.cs:39-74`
- Concrete enemies (`Goblin.cs`, `Bat.cs`) override state handlers
- Player detection uses `detectionRange` and `attackRange` fields

### Singleton Managers

Core systems use singleton pattern with lazy initialization:

- `GameManager` - Handles player death and scene reloading
- `CurrencyManager` - Tracks currency collection and persistence
- `HitStopManager` - Provides hitstop effect for combat feedback
- `InputManager` - Centralizes input handling with fallback support

Pattern: `public static ClassName Instance { get; private set; }`

### Player Movement System

`PlayerController.cs` implements advanced platformer mechanics:

- **Coyote time**: Grace period after leaving ground (line 15, 82-83)
- **Jump buffering**: Early jump input buffering (line 16, 88-89)
- **Dash/Roll**: Short burst movement with cooldown (line 11-12, 117-127)
- Ground detection uses `Physics2D.OverlapCircle` with configurable `LayerMask` (line 136-139)

### Combat System

Combat uses Unity's trigger collision system:

- `PlayerAttack.cs` activates hitbox collider temporarily
- Damage dealt via `OnTriggerEnter2D` checking for `EnemyBase` component
- `HitStopManager.Instance.TriggerHitStop()` provides impact feel
- Screen shake via `CameraShake.cs` attached to Cinemachine virtual camera

### Currency and Progression

Roguelite currency system:

- Enemies drop currency on death (configurable chance and amount in `EnemyBase.cs:11-19`)
- `CurrencyManager` persists currency via `PlayerPrefs`
- `CurrencyPickup.cs` handles collection triggers
- Currency resets on player death (`GameManager.cs:16-20`)

### Level Generation

`RoomGenerator.cs` provides sequential room spawning:

- Rooms spawned horizontally with configurable spacing
- Supports start room, standard rooms, and boss room
- Room prefabs should be designed with consistent width (20 units recommended)
- Can be extended to support procedural room selection

## Layer Configuration

Required physics layers:

- **Default** (0): General objects
- **Ground** (8): Platforms and tilemap colliders
- **Player** (9): Player character
- **Enemies** (10): Enemy characters
- **Pickups** (11): Currency and collectibles (triggers only)

Configure layer collision matrix in Edit > Project Settings > Physics2D.

## Development Workflow

### Scene Setup

The project requires manual scene setup as prefabs are not included in the repository. Follow `SETUP_GUIDE.md` for complete scene assembly instructions.

Key setup steps:
1. Create player GameObject with `PlayerController`, `PlayerAttack`, `PlayerHealth`
2. Add `GroundCheck` child transform positioned at player's feet
3. Create `AttackHitbox` child with trigger collider
4. Set up Cinemachine virtual camera with `CinemachineSetup` script
5. Create manager GameObject with `GameManager`, `CurrencyManager`, `HitStopManager`, `InputManager`

### Ground Detection Implementation

`PlayerController.IsGrounded()` at line 136 uses `Physics2D.OverlapCircle`:

```csharp
private bool IsGrounded()
{
    if (groundCheck == null) return false;
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
}
```

Ensure `groundCheck` transform, `groundCheckRadius`, and `groundLayerMask` are properly configured.

### Adding New Enemy Types

1. Create class inheriting from `EnemyStateMachine`
2. Override state handler methods: `HandlePatrolState`, `HandleAggroState`, `HandleAttackState`
3. Implement movement logic (walking, flying, etc.)
4. Configure detection ranges and attack parameters
5. Example: `Goblin.cs` (melee, ground-based) and `Bat.cs` (ranged, flying)

### Input System Integration

When adding new input actions:

1. Add property to `InputManager.cs`
2. Implement both new Input System path and legacy fallback
3. Reference pattern at `InputManager.cs:20-60`
4. Access via `InputManager.Instance.ActionName`

## Common Gotchas

- **Missing TextMeshPro**: Use `SimpleCurrencyDisplay.cs` or `SafeCurrencyUI.cs` instead of `CurrencyUI.cs`
- **Ground detection failing**: Verify `groundCheck` transform is assigned and on correct layer
- **Input not responding**: Ensure `InputManager` exists in scene and packages are installed
- **Camera not following**: Check `CinemachineSetup` configuration and player tag ("Player")
- **No currency drops**: Verify `currencyPickupPrefab` is assigned on enemy instances
- **Enemies not detecting player**: Player must have "Player" tag

## File References

Key files to understand the architecture:

- `PlayerController.cs:136-139` - Ground detection implementation
- `EnemyStateMachine.cs:39-74` - State machine core logic
- `InputManager.cs:20-60` - Input abstraction pattern
- `CurrencyManager.cs` - Singleton manager pattern
- `EnemyBase.cs:45-61` - Currency drop system
- `GameManager.cs:14-23` - Death and restart logic

## Unity Editor Extensions

- `AttackDebugger.cs` - Visualizes attack hitboxes in Scene view
- `EnemyHealthDebugger.cs` - Displays enemy health above sprites
- `AttackVisualizer.cs` - Shows attack range gizmos
- `QuickPlayerSetup.cs` - Helper script for rapid player prefab setup
