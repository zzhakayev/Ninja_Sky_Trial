# NINJA SKY TRIAL — Unity 6 Game Development Project

## 1. Project Overview

**Ninja Sky Trial** is a 3D action-platformer game developed in Unity 6.3 LTS for the Game Development Fundamentals course.

The player controls a ninja who must jump across floating platforms, collect energy coins, avoid dangerous obstacles and enemies, and defeat the final boss known as the Guardian.

The project includes three playable levels, a main menu, a victory screen, mobile controls, a health system, and a functional pause menu.

The interface is designed for portrait mobile screens and supports different screen sizes.

## 2. Engine and Technologies

- **Game Engine:** Unity 6.3 LTS (6000.3.24f1)
- **Programming Language:** C#
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Game Type:** 3D Action Platformer
- **Input:** Unity Input System
- **Character:** Rigged Mixamo Ninja
- **UI:** Unity Canvas, TextMeshPro, and IMGUI
- **Target Platform:** Mobile devices
- **Screen Orientation:** Portrait
- **Testing Tools:** Unity Editor and Device Simulator
- **Version Control:** Git and GitHub

## 3. Game Features

### Gameplay

- Three levels with increasing difficulty
- Floating and moving platforms
- Coins and collectible energy
- Enemy patrols and hazardous obstacles
- Jumping and shooting mechanics
- Player health and damage system
- Final boss battle against the Guardian
- Level progression through portals
- Victory screen after completing the game

### User Interface

- Main Menu with Start Game, How to Play, Settings, and Quit
- HUD displaying player HP, collected coins, and level number
- Virtual joystick for mobile movement
- JUMP and FIRE buttons
- PAUSE button on all three gameplay levels
- Pause Menu with Resume, Restart, and Main Menu
- Responsive layout for different screen resolutions
- Safe Area support for devices with notches and rounded corners

## 4. Scenes and Game Progression

The game contains five main scenes.

### Main Menu

`Assets/Scenes/Main menu.unity`

The player can start the game, view instructions, access settings, or quit.

### Level 1 — The Beginning

`Assets/Scenes/Level1.unity`

The player must jump across platforms, avoid hazards and enemies, collect at least two energy coins, and reach the cyan portal.

### Level 2 — Moving Platforms

`Assets/Scenes/Level2.unity`

The second level introduces additional obstacles and moving platforms. The player must collect at least three energy coins and reach the portal.

### Level 3 — Guardian Boss

`Assets/Scenes/Level3.unity`

The final level features a boss battle against the Guardian.

The player must avoid enemy attacks, use the FIRE mechanic to damage the boss, and reach the portal after defeating the Guardian.

The Guardian has 8 HP.

### Victory Screen

`Assets/Scenes/Win.unity`

After completing all three levels, the player reaches the victory screen and can return to the main menu.

The active game flow is:

**Main Menu → Level 1 → Level 2 → Level 3 → Win**

The original `SampleScene.unity` is retained as a backup and is not part of the main game flow.

## 5. Controls

### Desktop Controls

| Key | Action |
|---|---|
| WASD / Arrow Keys | Move |
| Space | Jump |
| X / Left Mouse Button | Shoot |
| UI Pause Button | Pause game |

### Mobile Controls

| Control | Action |
|---|---|
| Virtual Joystick | Move the ninja |
| JUMP | Jump between platforms |
| FIRE | Shoot projectiles |
| PAUSE (II) | Open Pause Menu |
| RESUME | Continue playing |
| RESTART | Restart the current level |
| MAIN MENU | Return to the main menu |

## 6. Health and HUD System

The player's HUD displays:

- Current level
- Player health (HP)
- Collected coins
- Required number of coins

The gameplay HUD shows the player starting with **3/3 HP**.

During Level 3, the interface also displays:

- Guardian boss HP
- A visual boss health bar
- Boss battle instructions

The HUD was improved to make important information more readable on mobile screens.

## 7. Pause Menu System

A functional Pause Menu was implemented for all three gameplay levels.

The system includes four main actions:

**Pause Game**

Stops gameplay by setting `Time.timeScale = 0`.

**Resume Game**

Restores `Time.timeScale = 1` and continues gameplay.

**Restart Level**

Restores normal time and reloads the current scene.

**Main Menu**

Restores normal time and loads the main menu scene.

The Pause Menu was saved as a reusable Unity Prefab:

`Assets/Prefabs/PauseCanvas.prefab`

The same Prefab is used in Level1, Level2, and Level3.

## 8. Responsive UI and Safe Area

The game interface was adapted for portrait mobile screens.

The UI uses:

- Canvas Scaler
- RectTransform anchors
- TextMeshPro
- Safe Area support
- Different layouts for mobile screen sizes

The `SafeAreaFitter.cs` script uses `Screen.safeArea` to adjust UI elements to the available screen area.

The HUD and pause controls were checked using Unity Device Simulator, including an iPhone with a screen notch and an iPad layout.

The responsive UI is intended to prevent important interface elements from being hidden behind device cutouts or screen edges.

## 9. Gameplay Architecture

The project is organized into reusable C# components.

### Character and Gameplay

**Movement.cs**

Handles character movement, jumping, rotation, animation parameters, and interactions.

**Health.cs**

Handles player health, damage, temporary invulnerability, and game-over behaviour.

**Shooter.cs and Bullet.cs**

Implement shooting, projectiles, and damage.

**MidtermLevel.cs**

Creates the different platforming levels using reusable 3D objects at runtime. It also manages objectives, HUD updates, audio, and level progression.

### Enemies and Obstacles

**BossEnemy.cs and BossProjectile.cs**

Control Guardian health, projectile attacks, and boss defeat.

**PatrolEnemy.cs**

Controls enemy patrol behaviour.

**DamageZone.cs**

Handles damage from dangerous objects.

**PlatformClipMotion.cs / MovingPlatform.cs**

Support moving-platform gameplay.

**SpinPickup.cs**

Controls collectible animation.

### Interface and Navigation

**NinjaModernMenu.cs**

Manages the modern main menu interface.

**PauseMenu.cs**

Controls pausing, resuming, restarting, and returning to the main menu.

**SafeAreaFitter.cs**

Adjusts UI layout to account for device safe areas and screen changes.

**VictoryScreen.cs**

Manages the final victory interface.

### Animation

`NInjaAnimator.controller`

Controls the Ninja's Idle, Run, and Jump animations using movement and grounded parameters.

## 10. Runtime Level Generation

An important part of this project is that Level1, Level2, and Level3 construct their gameplay environments at runtime.

Before entering Play mode, Unity may display the original template arena in Scene View.

When Play mode starts, `MidtermLevel.cs` creates the corresponding level environment and disables the original arena.

This is intentional behaviour, not a missing level.

## 11. Art and Audio

The project includes:

- A rigged Mixamo Ninja character
- 3D platforms and floating obstacles
- Coins and energy pickups
- Enemy models and boss objects
- Bright cyan, blue, and purple visual elements
- Character animations
- Sound effects and background music

Audio includes shooting, pickup, damage, jumping, boss-hit, and finish effects.

LevelMusic and BossMusic are stored with the project's audio resources and were synthesized for the project without third-party song samples.

## 12. How to Run the Project

1. Clone or download the GitHub repository.
2. Open **Unity Hub**.
3. Select **Add project from disk**.
4. Choose the downloaded project folder.
5. Open it with **Unity Editor 6000.3.24f1**.
6. Allow Unity to import all assets and packages.
7. Open `Assets/Scenes/Main menu.unity`.
8. Press **Play**.
9. Click **START GAME**.
10. Complete all three levels to reach the victory screen.

For mobile interface testing, open:

**Window → General → Device Simulator**

Select the desired device and use portrait orientation.

The game can also be tested with desktop controls in the Unity Editor.

## 13. Project Folder Structure

```text
Ninja_Sky_Trial/
├── Assets/
│   ├── Animations/
│   ├── Audio/
│   ├── Models/
│   ├── Prefabs/
│   │   └── PauseCanvas.prefab
│   ├── Resources/
│   ├── Scenes/
│   │   ├── Main menu.unity
│   │   ├── Level1.unity
│   │   ├── Level2.unity
│   │   ├── Level3.unity
│   │   └── Win.unity
│   └── Script/
│       ├── Movement.cs
│       ├── Health.cs
│       ├── Shooter.cs
│       ├── MidtermLevel.cs
│       ├── PauseMenu.cs
│       └── SafeAreaFitter.cs
├── Packages/
├── ProjectSettings/
├── README.md
└── AI_LOG.txt
```

## 14. Testing Status

The project has been tested in the Unity Editor.

Completed checks include:

- Main Menu navigation
- Level1 gameplay
- Level2 gameplay
- Level3 boss battle
- Victory screen flow
- Player movement and jumping
- Shooting and coin collection
- Pause and Resume
- Restart Level
- Return to Main Menu
- HUD readability improvements
- Device Simulator checks for different mobile screen layouts

The interface has been inspected in portrait mode using standard phone layouts, iPhone 12 Pro Max, and iPad simulations.

The game was tested on a physical iPhone and in the Unity Editor. The latest Pause Menu and responsive UI improvements were additionally tested using Unity Device Simulator.


## 15. AI Assistance

AI tools, including ChatGPT, were used during development for:

- C# coding assistance
- Debugging and troubleshooting
- Gameplay feature implementation
- Pause Menu development
- Responsive UI and Safe Area adjustments
- Documentation and project planning

AI-assisted changes were reviewed and tested through the Unity Editor by the student.

Details of AI usage are documented in `AI_LOG.txt`.

## 16. Version Control

The project uses Git and GitHub for version control.

Development work includes a dedicated feature branch:

`feature/mobile-ui`

The repository contains Unity scenes, source code, prefabs, and project configuration files.

The Unity `.gitignore` is used to exclude generated and temporary project files.

## 17. Course and Submission

**Course:** Game Development Fundamentals

**Project:** Ninja Sky Trial

**Assignment:** UI Development and Game Design Documentation (Assignment 8–9 / RK2)

The submission materials include the Unity project, GitHub repository, game design document (GDD), interface screenshots, AI usage log, and a narrated demonstration video.

This project is an educational game prototype developed for academic assessment.