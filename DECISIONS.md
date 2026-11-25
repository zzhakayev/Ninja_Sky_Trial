# DECISIONS.md

## Week 10 — Enemy AI

1. I used ChatGPT to learn how to implement three different enemy types using finite state machines: Patroller, Shooter, and Charger.
2. During development, enemies were positioned incorrectly because the level platforms were generated at runtime. I fixed their placement and NavMesh setup.
3. I tested Level 1 with two players. Tester 1 completed it in 2 attempts, while Tester 2 needed 6 attempts.
4. After testing, I reduced Shooter View Range from 8 to 5.5 and increased Shooter Cooldown from 1.5 to 2.2 seconds to make attacks fairer.
5. I tested the updated game and found it easier to play. I reviewed the AI-assisted code and kept the changes that suited my game.

## Week 11 — Data and Saves

1. I used ChatGPT to help implement and debug the JSON save system, ScriptableObjects, and level progression in Ninja Sky Trial.
2. During testing, the main menu buttons and player movement stopped working because Unity's old and new input systems were configured inconsistently.
3. I fixed the EventSystem configuration to use Input System UI Input Module with the New Input System. I tested the updated game on my Android phone.
4. I tested saving and loading progress by unlocking Level 2, closing the application, and reopening it. The unlocked level and audio settings were successfully restored.
5. I tested the Reset Progress feature and demonstrated changing enemy movement speed through a ScriptableObject without modifying the code.
6. I tested corrupted JSON save data by intentionally modifying the save.json file. The game handled the invalid data without crashing and loaded default values. After testing, I restored the original save file.

## Week 12 — Sound Pass

1. I created GameMixer with Master, Music, SFX and Tracks groups, Normal and Paused snapshots, and exposed MusicVol and SFXVol.
2. I asked ChatGPT to review the audio implementation. The old music sources were created again each time a scene loaded, so I chose a persistent GameAudio service with two music sources and a crossfade.
3. I kept the Week 11 JSON system and added separate SFX volume with backward-compatible loading of version 1 saves.
4. I chose distinct synthesized warning cues for each enemy, with small random pitch variation on repeated effects and separate menu/level tracks.
5. Week 12 audio features were tested successfully in Unity and on an Android device. Music crossfade, sound effects, pause snapshots, and volume persistence work correctly.
