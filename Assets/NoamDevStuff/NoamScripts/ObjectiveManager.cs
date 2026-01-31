using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = System.Random;

public class ObjectiveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> objectivePrefabs;
    [SerializeField] private List<Transform> objectiveSpawnPoints;

    
    private List<Objective> _objectives = new();
    void Start()
    {
        foreach (var t in objectivePrefabs)
        {
            Debug.Log("SpawnedObjective");
            var r = new Random();
            var randVal = r.Next(0, objectiveSpawnPoints.Count);
            var inst = Instantiate(t, objectiveSpawnPoints[randVal].position , objectiveSpawnPoints[randVal].rotation);
            _objectives.Add(inst.GetComponent<Objective>());
        }
    }
    public void OnResetDay()
    {
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
