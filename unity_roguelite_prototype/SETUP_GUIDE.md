# Unity Roguelite Prototype - Setup Guide

## Overview
This enhanced roguelite prototype includes all the core systems requested: player movement with coyote time, FSM-based enemy AI, combat with hitstop and screen shake, currency progression, level generation, Input System support, and Cinemachine integration.

## Unity Setup Requirements

### Project Configuration
1. **Unity Version**: 2022.3 LTS or later
2. **Render Pipeline**: URP (Universal Render Pipeline)
3. **2D Renderer**: Ensure URP is configured for 2D

### Required Packages
Install these packages via Window > Package Manager:
- **Universal RP** (com.unity.render-pipelines.universal)
- **2D Tilemap Extras** (com.unity.2d.tilemap.extras)
- **Input System** (com.unity.inputsystem)
- **Cinemachine** (com.unity.cinemachine)
- **2D Animation** (com.unity.2d.animation) - optional for character animations

## Scene Setup

### 1. Camera Setup
1. Delete the default Main Camera
2. Create an empty GameObject named "CM vcam1"
3. Add **CinemachineVirtualCamera** component
4. Add the **CinemachineSetup** script
5. Configure camera bounds and follow settings

### 2. Player Setup
1. Create a Player GameObject with:
   - **SpriteRenderer** (assign your player sprite)
   - **Rigidbody2D** (Gravity Scale: 3, Freeze Rotation Z: true)
   - **Collider2D** (adjust size to sprite)
   - **PlayerController** script
   - **PlayerAttack** script
   - **PlayerHealth** script

2. Create a child GameObject "GroundCheck":
   - Position it at the player's feet
   - Assign this to PlayerController's Ground Check field

3. Create a child GameObject "AttackHitbox":
   - Add **Collider2D** set as Trigger
   - Position it in front of the player
   - Assign to PlayerAttack's Attack Hitbox field
   - Disable the collider by default

### 3. Ground Setup
1. Create Tilemap GameObjects:
   - GameObject > 2D Object > Tilemap > Rectangular
   - Paint your ground tiles
   - Set the layer to "Ground" (create if needed)
2. Configure PlayerController's Ground Layer Mask to include "Ground"

### 4. Enemy Setup
For **Goblin** enemies:
1. Create GameObject with:
   - **SpriteRenderer**
   - **Rigidbody2D** (Gravity Scale: 1, Freeze Rotation Z: true)
   - **Collider2D**
   - **Goblin** script
2. Configure detection and attack ranges in inspector

For **Bat** enemies:
1. Create GameObject with:
   - **SpriteRenderer**
   - **Rigidbody2D** (Gravity Scale: 0 for flying)
   - **Collider2D**
   - **Bat** script
2. Position higher than ground level

### 5. Currency System Setup
1. Create a **CurrencyPickup** prefab:
   - **SpriteRenderer** (coin/gem sprite)
   - **Collider2D** set as Trigger
   - **CurrencyPickup** script
2. Assign this prefab to enemies' Currency Pickup Prefab field

### 6. UI Setup
1. Create Canvas (Screen Space - Overlay)
2. Add TextMeshPro text element for currency display
3. Add **CurrencyUI** script to a GameObject
4. Assign the text component to the script

### 7. Game Manager Setup
1. Create empty GameObject "GameManager"
2. Add **GameManager** script
3. Add **CurrencyManager** script
4. Add **HitStopManager** script
5. Add **InputManager** script (if using Input System)

### 8. Level Generation Setup
1. Create room prefabs (Tilemap-based recommended):
   - Design 3-4 different room layouts
   - Ensure consistent width (20 units recommended)
   - Include enemy spawn points
2. Create empty GameObject "LevelGenerator"
3. Add **RoomGenerator** script
4. Assign room prefabs to appropriate arrays

## Layer Setup
Recommended layers:
- **Default** (0): General objects
- **Ground** (8): Ground tiles and platforms
- **Player** (9): Player character
- **Enemies** (10): Enemy characters
- **Pickups** (11): Currency and items

## Physics Settings
1. Edit > Project Settings > Physics2D
2. Configure layer collision matrix:
   - Player should collide with Ground and Enemies
   - Enemies should collide with Ground and Player
   - Pickups should not collide with anything (triggers only)

## Input System Configuration
If using the new Input System:
1. Create Input Actions asset or use the built-in **InputManager**
2. Configure bindings:
   - **Move**: WASD/Arrow Keys + Left Stick
   - **Jump**: Space + A Button (gamepad)
   - **Attack**: Left Click/Ctrl + X Button (gamepad)
   - **Dash**: Left Shift + B Button (gamepad)

## Testing Checklist
- [ ] Player moves left/right smoothly
- [ ] Jump works with coyote time
- [ ] Dash/roll functions properly
- [ ] Ground detection works correctly
- [ ] Attack triggers hitbox and effects
- [ ] Enemies patrol and aggro properly
- [ ] Combat feels responsive with hitstop
- [ ] Screen shake occurs on hits
- [ ] Currency drops and is collectible
- [ ] UI displays currency correctly
- [ ] Camera follows player smoothly
- [ ] Level generation creates rooms

## Common Issues

### Ground Detection Not Working
- Ensure GroundCheck transform is assigned
- Check Ground Layer Mask settings
- Verify ground objects are on correct layer

### Input Not Responding
- Check if InputManager is in scene
- Verify Input System package is installed
- Ensure player scripts reference InputManager correctly

### Camera Not Following
- Assign player transform to Cinemachine Follow field
- Check CinemachineSetup script configuration
- Ensure player has "Player" tag

### No Currency Drops
- Assign CurrencyPickup prefab to enemies
- Check currency drop chance percentage
- Ensure CurrencyManager is in scene

## Extensions and Improvements
The prototype provides a solid foundation for:
- Animation integration (Animator + 2D Animation)
- Audio system (attack sounds, music)
- Particle effects (hit sparks, currency shimmer)
- More complex enemy behaviors
- Power-ups and weapon variety
- Save system for meta-progression
- Mobile input optimization
- Performance optimization for large levels

This setup creates a fully functional roguelite prototype matching the Dead Cells-inspired specifications with modern Unity best practices.