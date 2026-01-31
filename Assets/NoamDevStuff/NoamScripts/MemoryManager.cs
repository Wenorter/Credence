using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct TagValPair
{
    [TagSelector] public string tag;  // tag
    public float value;               // lifetime seconds
}

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; }

    [Header("Memory Settings (per tag)")]
    [Tooltip("Used if a tag is not found in Tag Lifetimes list.")]
    [SerializeField] private float defaultMemoryLifetime = 20f;

    [Tooltip("Per-tag memory lifetime in seconds (key = GameObject tag, value = seconds).")]
    [SerializeField] private List<TagValPair> tagLifetimes = new List<TagValPair>();

    [Header("Ghost Drift / Fade")]
    public float driftMaxMeters = 1.2f;         // how far ghosts can drift at low confidence
    public float driftSpeed = 0.25f;            // how fast drift moves
    public float minStrengthToWrite = 0.15f;    // don’t write super-weak observations
    public float fadeExponent = 2f;

    [Header("Ghost Look")]
    public Material ghostMaterial;              // your white aura material (transparent/emissive)

    private class Record
    {
        public Memorable src;
        public string tag;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public float lastSeenTime;
        public float confidence; // 1..0
        public float seed;
        public GhostInstance ghost;

        // NEW: cap the max fade by Memorable's color alpha
        public float maxAlpha; // 0..1
    }

    private readonly Dictionary<string, Record> _mem = new Dictionary<string, Record>();

    // Runtime lookup: tag -> lifetime
    private readonly Dictionary<string, float> _tagLifetimeLookup = new Dictionary<string, float>();

    private void Awake()
    {
        Instance = this;
        RebuildTagLookup();
    }

    private void OnValidate()
    {
        RebuildTagLookup();
    }

    private void RebuildTagLookup()
    {
        _tagLifetimeLookup.Clear();

        for (int i = 0; i < tagLifetimes.Count; i++)
        {
            string tag = tagLifetimes[i].tag;
            float time = tagLifetimes[i].value;

            if (string.IsNullOrWhiteSpace(tag))
                continue;

            _tagLifetimeLookup[tag] = time;
        }
    }

    private float GetLifetimeForTag(string tag)
    {
        if (!string.IsNullOrEmpty(tag) && _tagLifetimeLookup.TryGetValue(tag, out float t))
            return t;

        return defaultMemoryLifetime;
    }

    public void FlushMemory()
    {
        foreach (var kv in _mem)
        {
            var rec = kv.Value;
            rec?.ghost?.Destroy();
        }

        _mem.Clear();
    }

    public void Observe(Memorable m, float strength, bool isFading = true)
    {
        if (!m || strength < minStrengthToWrite) return;

        if (!_mem.TryGetValue(m.Guid, out var rec))
        {
            rec = new Record
            {
                src = m,
                tag = m.gameObject.tag,
                seed = Random.value * 1000f,
                confidence = 1f,
                lastSeenTime = Time.time,

                // NEW: cache max alpha from memorable
                maxAlpha = Mathf.Clamp01(m.color.a),
            };
            _mem[m.Guid] = rec;

            rec.ghost = GhostInstance.Create(ghostMaterial, m, m.color, m.layer, isFading);
        }
        else
        {
            rec.tag = m.gameObject.tag;

            // NEW: keep alpha cap updated in case you change Memorable.color at runtime
            rec.maxAlpha = Mathf.Clamp01(m.color.a);
        }

        rec.pos = m.transform.position;
        rec.rot = m.transform.rotation;
        rec.scale = m.transform.lossyScale;
        rec.lastSeenTime = Time.time;

        rec.confidence = Mathf.Clamp01(Mathf.Max(rec.confidence, strength));

        rec.ghost.SetBaseTransform(rec.pos, rec.rot, rec.scale);
    }

    private void Update()
    {
        float now = Time.time;
        var dead = new List<string>();

        foreach (var kv in _mem)
        {
            var rec = kv.Value;

            float lifetime = GetLifetimeForTag(rec.tag);
            if (lifetime <= 0f)
            {
                rec.ghost.Destroy();
                dead.Add(kv.Key);
                continue;
            }

            float age = now - rec.lastSeenTime;
            float t = Mathf.Clamp01(age / Mathf.Max(0.001f, lifetime));
            rec.confidence = 1f - t;

            if (rec.confidence <= 0.001f)
            {
                rec.ghost.Destroy();
                dead.Add(kv.Key);
                continue;
            }

            float driftAmp = driftMaxMeters * (1f - rec.confidence);

            float n1 = Mathf.PerlinNoise(rec.seed, now * driftSpeed);
            float n2 = Mathf.PerlinNoise(rec.seed + 31.7f, now * driftSpeed);
            float n3 = Mathf.PerlinNoise(rec.seed + 99.1f, now * driftSpeed);

            Vector3 drift = new Vector3(n1 - 0.5f, (n2 - 0.5f) * 0.3f, n3 - 0.5f) * (2f * driftAmp);

            // NEW: fade is capped by Memorable.color.a (maxAlpha)
            // Base fade from confidence curve:
            float baseFade = Mathf.Pow(rec.confidence, fadeExponent);

            // Prefer live src alpha if still valid, else cached:
            float maxAlpha = rec.src ? Mathf.Clamp01(rec.src.color.a) : rec.maxAlpha;

            float fade = Mathf.Clamp01(baseFade * maxAlpha);

            rec.ghost.UpdateGhost(drift, fade, Time.deltaTime);
        }

        foreach (var k in dead)
            _mem.Remove(k);
    }
}
