using System.Collections.Generic;
using UnityEngine;
using System;
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


    private List<EnemyNavAI> _enemies = new();


    void Start()
    {
        for(var i1 = 0 ; i1 < enemyAmountPerType.Length ; i1++)
        {
            for(var i2 = 0 ; i2 < enemyAmountPerType[i1] ; i2++)
            {
                Debug.Log("SpawnedEnemy");
                var r = new Random();
                var inst = Instantiate(enemyPrefabs[i1], enemySpawnPoints[r.Next(0 , enemySpawnPoints.Count)].position , quaternion.identity);
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
