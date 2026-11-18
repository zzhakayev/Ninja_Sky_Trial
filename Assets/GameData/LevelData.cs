
using System;
using System.Collections.Generic;
using UnityEngine;

public enum LevelGoal
{
    CollectCoins,
    ReachExit,
    DefeatBoss,
    Survive
}

[Serializable]
public class EnemyWaveData
{
    public GameObject enemyPrefab;
    public int count;
    public float delay;
    public Vector3[] spawnPoints;
}

[CreateAssetMenu(
    fileName = "LevelData",
    menuName = "Game/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int levelNumber;

    [Header("Goal")]
    public LevelGoal goal;
    public int goalAmount;
    public float timeLimit;

    [Header("Player")]
    public Vector3 playerSpawnPoint;

    [Header("Enemies")]
    public List<EnemyWaveData> waves = new List<EnemyWaveData>();
}
