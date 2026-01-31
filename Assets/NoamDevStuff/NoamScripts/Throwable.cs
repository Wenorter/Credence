using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField, LayerSelector] private List<int> goodLayers;

    [Header("Impact Check")]
    [Tooltip("Target impact speed to compare against (your throw value is 14).")]
    [SerializeField] private float expectedImpactSpeed = 14f;

    [Tooltip("How close it must be to count. Example: 3 means 11..17 counts as 'close'.")]
    [SerializeField] private float impactSpeedTolerance = 3f;

    [Tooltip("Ignore tiny bumps.")]
    [SerializeField] private float minImpactSpeed = 1f;

    private EnemyManager _enemyMan;

    private void Start()
    {
        _enemyMan = FindFirstObjectByType<EnemyManager>();
        if (_enemyMan == null)
            Debug.LogWarning("Throwable: EnemyManager not found in scene.", this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");

        // "How hard did it hit?" (m/s). This is the best match for your "14" throw value.
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < minImpactSpeed)
            return;

        bool closeTo14 = Mathf.Abs(impactSpeed - expectedImpactSpeed) <= impactSpeedTolerance;
        if (!closeTo14)
            return;

        int collLay = collision.gameObject.layer;
        
        for (int i = 0; i < goodLayers.Count; i++)
        {
            if (collLay == goodLayers[i])
            {
                Debug.Log($"Coll Layer Check Worked! ImpactSpeed={impactSpeed:0.00}");

                if (_enemyMan != null)
                    _enemyMan.OnThrowableHeard(transform);

                break;
            }
        }
    }
}