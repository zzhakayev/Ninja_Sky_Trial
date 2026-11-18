using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Game/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)] public float moveSpeed;

    [Header("Health")]
    [Min(1)] public int maxHP;

    [Header("Combat")]
    [Min(0)] public int damage;
    [Min(0f)] public float viewRange;
    [Min(0f)] public float attackRange;
    [Min(0f)] public float attackCooldown;
    [Min(0f)] public float windUpTime;

    [Header("Charger Settings")]
    [Min(0.1f)] public float dashDistance = 5f;
    [Min(0f)] public float stunDuration = 1f;
}
