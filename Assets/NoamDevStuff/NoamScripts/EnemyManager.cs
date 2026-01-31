using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = System.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [Header("Settings")]
    [Tooltip("How many Enemies to Spawn , index = enemy type (enemy prefab index)")]
    [SerializeField] private int[] enemyAmountPerType;
    [SerializeField] private bool isSpawning = true;

    private List<EnemyNavAI> _enemies = new();


    void Start()
    {
        if (!isSpawning) return;
        
        for(var i1 = 0 ; i1 < enemyAmountPerType.Length ; i1++)
        {
            for(var i2 = 0 ; i2 < enemyAmountPerType[i1] ; i2++)
            {
                Debug.Log("SpawnedEnemy");
                var r = new Random();
                var randVal = r.Next(0, enemySpawnPoints.Count);
                var inst = Instantiate(enemyPrefabs[i1], enemySpawnPoints[randVal].position , enemySpawnPoints[randVal].rotation);
                _enemies.Add(inst.GetComponent<EnemyNavAI>());
            }    
        }
    }
    public void OnResetDay()
    {
        foreach (var e in _enemies)
        {
            var r = new Random();
            e.OnResetDay(enemySpawnPoints[r.Next(0 , enemySpawnPoints.Count)]);
        }
    }

}
