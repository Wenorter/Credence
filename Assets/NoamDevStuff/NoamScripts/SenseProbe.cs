using System.Collections.Generic;
using UnityEngine;

public class SenseProbe : MonoBehaviour
{
    [Header("Sense Settings")]
    [Tooltip("Default max sense distance if tag not found in list.")]
    [SerializeField] public float defaultRadius = 4.0f;

    [Tooltip("Optional global cap (0 = no cap). If set > 0, physics query uses this max for performance.")]
    [SerializeField] private float globalMaxRadiusCap = 0f;

    [Tooltip("Per-tag max distance to sense (tag dropdown).")]
    [SerializeField] private List<TagValPair> tagSeenDistances = new List<TagValPair>();

    public LayerMask senseMask;
    public bool requireLineOfSight = false;
    public LayerMask occluderMask;

    [Header("Update")]
    public float tickRate = 30f;

    private readonly Collider[] _hits = new Collider[256];
    private readonly HashSet<int> _seenThisTick = new HashSet<int>();
    private readonly Dictionary<int, Renderer> _active = new Dictionary<int, Renderer>();

    private readonly Dictionary<string, float> _tagDistanceLookup = new Dictionary<string, float>();

    private float _nextTick;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        RebuildTagLookup();
    }

    private void OnValidate()
    {
        RebuildTagLookup();
    }

    private void RebuildTagLookup()
    {
        _tagDistanceLookup.Clear();

        for (int i = 0; i < tagSeenDistances.Count; i++)
        {
            string tag = tagSeenDistances[i].tag;
            float dist = tagSeenDistances[i].value;

            if (string.IsNullOrWhiteSpace(tag))
                continue;

            _tagDistanceLookup[tag] = dist;
        }
    }

    private float GetSenseDistanceForTag(string tag)
    {
        if (!string.IsNullOrEmpty(tag) && _tagDistanceLookup.TryGetValue(tag, out float d))
            return d;

        return defaultRadius;
    }

    private float GetQueryRadius()
    {
        // Use the largest configured radius for OverlapSphere, for performance + correctness.
        float max = defaultRadius;

        for (int i = 0; i < tagSeenDistances.Count; i++)
            max = Mathf.Max(max, tagSeenDistances[i].value);

        if (globalMaxRadiusCap > 0f)
            max = Mathf.Min(max, globalMaxRadiusCap);

        return Mathf.Max(0f, max);
    }

    private void Update()
    {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + 1f / Mathf.Max(1f, tickRate);

        _seenThisTick.Clear();

        float queryRadius = GetQueryRadius();
        int count = Physics.OverlapSphereNonAlloc(transform.position, queryRadius, _hits, senseMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (!col) continue;

            var rend = col.GetComponentInParent<Renderer>();
            if (!rend) continue;

            int id = rend.GetInstanceID();
            _seenThisTick.Add(id);

            // Distance from probe to object
            float d = Vector3.Distance(transform.position, rend.bounds.ClosestPoint(transform.position));

            // Per-tag max distance
            string objTag = rend.gameObject.tag;
            float tagRadius = GetSenseDistanceForTag(objTag);

            // If this tag is configured to be "never sensed"
            if (tagRadius <= 0f)
                continue;

            // Outside this tag's sense distance
            if (d > tagRadius)
                continue;

            // Optional LOS check (only after passing distance, to avoid extra raycasts)
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

            // Strength based on *that tag's radius*
            float strength = 1f - Mathf.Clamp01(d / tagRadius);

            ApplySense(rend, strength);

            // Feed memory system
            var mem = rend.GetComponentInParent<Memorable>();
            if (mem && mem.ignoredProbeLayer != mem.layer)
                MemoryManager.Instance.Observe(mem, strength);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, GetQueryRadius());
    }
}
