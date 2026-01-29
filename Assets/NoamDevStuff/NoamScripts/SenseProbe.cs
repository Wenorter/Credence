using System.Collections.Generic;
using UnityEngine;

public class SenseProbe : MonoBehaviour
{
    [Header("Sense Settings")]
    public float radius = 4.0f;
    public LayerMask senseMask;
    public bool requireLineOfSight = false;
    public LayerMask occluderMask;

    [Header("Update")]
    public float tickRate = 30f;

    private readonly Collider[] _hits = new Collider[256];
    private readonly HashSet<int> _seenThisTick = new HashSet<int>();
    private readonly Dictionary<int, Renderer> _active = new Dictionary<int, Renderer>();

    private float _nextTick;
    private MaterialPropertyBlock _mpb;

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + 1f / Mathf.Max(1f, tickRate);

        _seenThisTick.Clear();

        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _hits, senseMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (!col) continue;

            var rend = col.GetComponentInParent<Renderer>();
            if (!rend) continue;

            int id = rend.GetInstanceID();
            _seenThisTick.Add(id);

            // Optional LOS check
            if (requireLineOfSight)
            {
                Vector3 target = rend.bounds.center;
                Vector3 dir = (target - transform.position);
                float dist = dir.magnitude;
                if (dist > 0.001f)
                {
                    if (Physics.Raycast(transform.position, dir / dist, dist, occluderMask, QueryTriggerInteraction.Ignore))
                        continue;
                }
            }

            float d = Vector3.Distance(transform.position, rend.bounds.ClosestPoint(transform.position));
            float strength = 1f - Mathf.Clamp01(d / radius);

            ApplySense(rend, strength);

            // Feed memory system
            var mem = rend.GetComponentInParent<Memorable>();
            if (mem && mem.ignoredProbeLayer != mem.layer) MemoryManager.Instance.Observe(mem, strength);
        }

        // Clear aura from things not sensed this tick
        var keys = new List<int>(_active.Keys);
        foreach (int id in keys)
        {
            if (_seenThisTick.Contains(id)) continue;
            ClearSense(_active[id]);
            _active.Remove(id);
        }
    }

    private void ApplySense(Renderer rend, float strength)
    {
        _active[rend.GetInstanceID()] = rend;

        rend.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Sense", strength);
        _mpb.SetFloat("_IsMemory", 0f);
        rend.SetPropertyBlock(_mpb);
    }

    private void ClearSense(Renderer rend)
    {
        rend.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Sense", 0f);
        _mpb.SetFloat("_IsMemory", 0f);
        rend.SetPropertyBlock(_mpb);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
