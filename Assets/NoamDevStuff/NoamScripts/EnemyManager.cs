using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PriestActions priestLogicRef;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [Header("Settings")]
    [Tooltip("How many Enemies to Spawn , index = enemy type (enemy prefab index)")]
    [SerializeField] private int[] enemyAmountPerType;
    [SerializeField] private bool isSpawning = true;

    [Header("Throwable Hearing")]
    [Tooltip("If a throwable happens within this distance from a demon, it will investigate.")]
    [SerializeField] private float throwableHearDistance = 10f;

    private readonly List<EnemyNavAI> _enemies = new();

    private void Start()
    {
        if (!isSpawning) return;

        for (var i1 = 0; i1 < enemyAmountPerType.Length; i1++)
        {
            for (var i2 = 0; i2 < enemyAmountPerType[i1]; i2++)
            {
                Debug.Log("SpawnedEnemy");

                var r = new Random();
                var randVal = r.Next(0, enemySpawnPoints.Count);

                var inst = Instantiate(
                    enemyPrefabs[i1],
                    enemySpawnPoints[randVal].position,
                    enemySpawnPoints[randVal].rotation
                );

                var instNav = inst.GetComponent<EnemyNavAI>();
                if (instNav != null)
                {
                    instNav.priestLogic = priestLogicRef;
                    _enemies.Add(instNav);
                }
                else
                {
                    Debug.LogWarning($"{inst.name} spawned but has no EnemyNavAI component.");
                }
            }
        }
    }

    public void OnResetDay()
    {
        foreach (var e in _enemies)
        {
            var r = new Random();
            e.OnResetDay(enemySpawnPoints[r.Next(0, enemySpawnPoints.Count)]);
        }
    }

    // NEW
    public void OnThrowableHeard(Transform throwable)
    {
        if (throwable == null) return;

        float hearDist = Mathf.Max(0f, throwableHearDistance);
        float hearDistSqr = hearDist * hearDist;

        for (int i = 0; i < _enemies.Count; i++)
        {
            var demon = _enemies[i];
            if (!demon) continue;

            float dSqr = (demon.transform.position - throwable.position).sqrMagnitude;
            if (dSqr <= hearDistSqr)
            {
                demon.OnThrowableHeard(throwable);
            }
        }
    }
}
