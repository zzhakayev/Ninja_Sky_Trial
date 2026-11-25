
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    // Automatically create the manager when the game starts.
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateManager()
    {
        if (Instance != null)
            return;

        GameObject manager = new GameObject("SaveManager");
        manager.AddComponent<SaveManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this object when changing scenes.
        DontDestroyOnLoad(gameObject);

        Data = SaveSystem.Load();

        Debug.Log("SaveManager initialized!");
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return Data != null &&
               Data.unlockedLevels.Contains(levelNumber);
    }

    public void CompleteLevel(
        int levelNumber,
        int score,
        int coins)
    {
        if (Data == null || levelNumber < 1 || levelNumber > 3)
            return;

        Data.lastCompletedLevel = Mathf.Max(
            Data.lastCompletedLevel,
            levelNumber
        );

        Data.bestScore = Mathf.Max(
            Data.bestScore,
            score
        );

        Data.totalCoins += Mathf.Max(0, coins);

        // Unlock the next level.
        if (levelNumber < 3)
        {
            int nextLevel = levelNumber + 1;

            if (!Data.unlockedLevels.Contains(nextLevel))
                Data.unlockedLevels.Add(nextLevel);
        }

        SaveSystem.Save(Data);

        Debug.Log("Level " + levelNumber +
                  " completed and saved!");
    }

    public void SetMusicVolume(float volume)
    {
        Data.musicVolume = Mathf.Clamp01(volume);
        SaveSystem.Save(Data);
        GameAudio.RefreshVolumes();
    }

    public void SetSfxVolume(float volume)
    {
        Data.sfxVolume = Mathf.Clamp01(volume);
        SaveSystem.Save(Data);
        GameAudio.RefreshVolumes();
    }

    public void SetMusicEnabled(bool enabled)
    {
        Data.musicEnabled = enabled;
        SaveSystem.Save(Data);
        GameAudio.RefreshVolumes();
    }

    public void SetVibration(bool enabled)
    {
        Data.vibrationEnabled = enabled;
        SaveSystem.Save(Data);
    }

    public void ResetProgress()
    {
        SaveSystem.Delete();
        Data = new SaveData();
        GameAudio.RefreshVolumes();

        Debug.Log("Progress reset!");
    }

    // Important for iPhone and other mobile devices.
    private void OnApplicationPause(bool paused)
    {
        if (paused && Instance == this && Data != null)
            SaveSystem.Save(Data);
    }
}
