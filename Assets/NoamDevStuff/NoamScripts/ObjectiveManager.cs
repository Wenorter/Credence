using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = System.Random;

public class ObjectiveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> objectivePrefabs;
    [SerializeField] private List<Transform> objectiveSpawnPoints;
    private int[][] objAmountPerDay;
    
    private List<Objective> _objectives = new();

    private void Awake()
    {
        objAmountPerDay = new int[][]
        {
            new int[] { 2, },   // day 0
            new int[] { 1, },   // day 1
            new int[] { 1  },   // day 2
        };
    }

    private void SpawnByDay(int currentDay)
    {
        for (var prefabVar = 0; prefabVar < objAmountPerDay[currentDay].Length ; prefabVar++)
        {
            for (var amountIndex = 0; amountIndex < objAmountPerDay[currentDay][prefabVar]; amountIndex++)
            {
                Debug.Log("SpawnedObjective");
                var r = new Random();
                var randVal = r.Next(0, objectiveSpawnPoints.Count);
                var inst = Instantiate(objectivePrefabs[prefabVar], objectiveSpawnPoints[randVal].position , objectiveSpawnPoints[randVal].rotation);
                _objectives.Add(inst.GetComponent<Objective>());
            }
        }
    }
    public void OnResetDay(int currentDay)
    {
        SpawnByDay(currentDay);
        Debug.Log(_objectives.Count);
        foreach (var o in _objectives)
        {
            var r = new Random();
            int random = r.Next(0, objectiveSpawnPoints.Count);
            Debug.Log(random);
            o.OnResetDay(objectiveSpawnPoints[random]);
        }
    }
}
