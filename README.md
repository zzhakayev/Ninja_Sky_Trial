# NINJA SKY TRIAL — Unity 6 Midterm Project

## Project overview
A 3D mobile-ready platformer with a rigged Mixamo Ninja character, three stages, jumping between platforms, collectibles, moving platforms, hazards, enemies, and a final boss.

## Engine and controls
- Unity: 6000.3.24f1 (Unity 6.3 LTS)
- Project render pipeline: Universal Render Pipeline (URP)
- Input: Unity Input System, PlayerInput and on-screen joystick/buttons
- Desktop: WASD / arrow keys to move, Space to jump, X to shoot (also left mouse button)
- Phone (Unity Remote 5): virtual left stick, JUMP and FIRE buttons

## Scenes and full flow
1. `Assets/Scenes/Main menu.unity` — press Start
2. `Assets/Scenes/Level1.unity` — jump across floating platforms, avoid drone and spikes, collect at least two energy coins, touch cyan portal
3. `Assets/Scenes/Level2.unity` — more hazards and a moving platform, collect at least three energy coins, enter portal
4. `Assets/Scenes/Level3.unity` — avoid red projectiles, shoot Guardian Boss eight times, touch the portal that appears
5. `Assets/Scenes/Win.unity` — victory and Back to Main Menu

The five active scenes are in `ProjectSettings/EditorBuildSettings.asset`. `SampleScene.unity` remains as a backup of the original assignment and is excluded from the play flow.

## Gameplay architecture
- `Movement.cs`: physics, jumping, rotation, Ninja animation parameters, coins, portals.
- `Health.cs`: five HP, blinking invulnerability after taking damage, restart on game over.
- `Shooter.cs` and `Bullet.cs`: projectiles, cooldown, damage dealt to boss.
- `MidtermLevel.cs`: constructs different platforming levels from reusable 3D primitives at runtime, drives goals/HUD/audio/scene progression.
- `BossEnemy.cs` and `BossProjectile.cs`: boss health, timed projectile attacks, defeat behavior.
- `PatrolEnemy.cs`, `DamageZone.cs`, `SpinPickup.cs`: enemy patrol, spikes, collectible animation.
- `VictoryScreen.cs`: final screen and return navigation.
- `NInjaAnimator.controller`: Idle/Run/Jump with `Speed` float and `isGrounded` bool.

**Important:** Level1, Level2 and Level3 deliberately use runtime level construction. Before entering Play mode, their Scene views still show the original template arena. During Play mode each generates its own distinct level and disables the legacy arena. This is intentional, not a missing scene.

## Audio
Original project sounds: Shoot, Pickup, Hurt. Added generated sounds: Jump, BossHit, Finish. Added original synthesized LevelMusic and BossMusic, stored in `Assets/Resources/Audio`. They were synthesized for this project with no third-party song samples.

## How to run
1. Unzip the whole project to a new folder.
2. Unity Hub → Add project from disk → choose the extracted project folder.
3. Open it with Unity Editor **6000.3.24f1**.
4. Allow package and FBX import to finish. Existing imported Mixamo animation clips are large and may take time to import.
5. Open `Assets/Scenes/Main menu.unity` and press Play. Press Start.
6. Verify Ninja's model and three animations. If Ninja does not appear, select Player in Level1 and check the `Midterm Level` component: the `Ninja Prefab` must point to `Assets/Models/Ninja.fbx`, and `Ninja Controller` to `Assets/Animations/NInjaAnimator.controller`.
7. After a quick PC test, connect the iPhone with Unity Remote 5 as configured in this project, and demonstrate the on-screen controls.

## Submission
Include Assets, Packages, ProjectSettings, this README and `AI_LOG.txt` in the final ZIP. Provide a separate narrated gameplay MP4 (~2–3 minutes), made locally in Unity using phone controls or the desktop, as required by the instructor. Do not include the Library folder.

## Testing status
Static project and scene structure were inspected and source-level changes were prepared outside the Unity Editor. This package has **not** been compiled, play-tested, or device-tested by the assistant. Validate in Unity and fix any Console errors before submitting. Mobile performance and phone demonstration remain the student's responsibility.
