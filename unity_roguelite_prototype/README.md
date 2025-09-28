# Unity Roguelite Prototype

This repository contains a bare‑bones prototype for a 2D roguelite game built with Unity.  It targets both desktop (Windows, macOS and Linux) and mobile (iOS and Android) platforms.  The code and file structure here are meant as a starting point for further development.

## Features

* **Movement and Jumping** – The `PlayerController` script implements left/right movement, jumping with coyote time and jump buffering, and a short dash.  You will need to add your own ground detection to the `IsGrounded()` method.
* **Combat** – The `PlayerAttack` script is a placeholder for melee attacks.  It includes a simple method for dealing damage via collision triggers.
* **Health System** – The `PlayerHealth` component keeps track of hit points and notifies the `GameManager` when the player dies.
* **Enemies** – An `EnemyBase` class provides a foundation for AI behaviour.  Two simple enemy scripts (`Goblin` and `Bat`) inherit from it.  They include rudimentary movement and attack logic and can be extended to add more complex behaviours.
* **Level Generation** – The `RoomGenerator` script demonstrates how to stitch together a list of room prefabs to create a simple level sequence.  You can replace the placeholder rooms with your own tilemap or prefab scenes.
* **Camera Shake** – A lightweight `CameraShake` component shows how to implement hit feedback via a simple oscillation.  For production projects you may wish to use Cinemachine’s built‑in camera shaking instead.

## Getting Started

1. Open the Unity Hub and create a new **2D URP** project (Unity 2022.3 LTS or later is recommended).
2. Copy the contents of the `unity_roguelite_prototype/Assets` folder into your project’s `Assets` folder.
3. Create the necessary prefabs for the player, enemies and rooms and assign the scripts contained here.
4. Implement `IsGrounded()` in `PlayerController` to use your chosen ground detection method (e.g. a `LayerMask` with `Physics2D.OverlapCircle` or `BoxCast`).
5. Replace the placeholder visuals with your own sprites, animations and tilemaps.  The `Art`, `Animations` and `UI` directories are included for organisational purposes.

This repository provides only the code scaffolding.  It is not a complete game but should help you bootstrap your own roguelite project.

## Package Compatibility

The codebase includes multiple UI options to handle different package availability:

- **CurrencyUI.cs** - Full-featured UI with TextMeshPro and UI package support
- **SimpleCurrencyDisplay.cs** - Basic display without package dependencies
- **SafeCurrencyUI.cs** - Ultra-safe version that works in any Unity setup

If you encounter compilation errors related to missing packages:

1. **For TextMeshPro errors**: Use `SimpleCurrencyDisplay` or `SafeCurrencyUI` instead of `CurrencyUI`
2. **For Input System errors**: The scripts include fallback to legacy input
3. **For Cinemachine errors**: The camera setup includes a simple follow camera fallback

The `PackageDefines.cs` script automatically detects available packages and sets appropriate compilation symbols.