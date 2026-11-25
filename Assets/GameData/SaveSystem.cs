
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int version = 2;

    public int bestScore = 0;
    public int totalCoins = 0;
    public int lastCompletedLevel = 0;

    public List<int> unlockedLevels =
        new List<int> { 1 };

    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
    public bool musicEnabled = true;
    public bool vibrationEnabled = true;
}

public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("New game: using default save.");
            return new SaveData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("Save file is empty.");

            SaveData data =
                JsonUtility.FromJson<SaveData>(json);

            if (data != null && data.version == 1)
            {
                data.version = 2;
                data.sfxVolume = 0.8f;
                data.musicEnabled = PlayerPrefs.GetInt("Music", 1) != 0;
            }

            if (data == null ||
                data.version != 2 ||
                data.unlockedLevels == null ||
                !data.unlockedLevels.Contains(1) ||
                data.bestScore < 0 ||
                data.totalCoins < 0 ||
                data.lastCompletedLevel < 0 ||
                data.musicVolume < 0f ||
                data.musicVolume > 1f ||
                data.sfxVolume < 0f ||
                data.sfxVolume > 1f)
            {
                throw new Exception("Invalid save data.");
            }

            Debug.Log("Game progress loaded.");
            return data;
        }
        catch (Exception error)
        {
            Debug.LogWarning(
                "Save file is broken. Using defaults: " +
                error.Message
            );

            return new SaveData();
        }
    }

    public static void Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Cannot save null data.");
            return;
        }

        try
        {
            Directory.CreateDirectory(
                Application.persistentDataPath
            );

            string json =
                JsonUtility.ToJson(data, true);

            File.WriteAllText(SavePath, json);

            Debug.Log("Game saved: " + SavePath);
        }
        catch (Exception error)
        {
            Debug.LogError(
                "Save failed: " + error.Message
            );
        }
    }

    public static void Delete()
    {
        try
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);

            Debug.Log("Save file deleted.");
        }
        catch (Exception error)
        {
            Debug.LogError(
                "Delete failed: " + error.Message
            );
        }
    }
}
