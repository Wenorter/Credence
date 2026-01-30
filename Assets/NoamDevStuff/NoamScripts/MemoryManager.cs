using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance { get; private set; }

    [Header("Memory Settings")]
    public float memoryLifetime = 20f;          // seconds until basically gone
    public float driftMaxMeters = 1.2f;         // how far ghosts can drift at low confidence
    public float driftSpeed = 0.25f;            // how fast drift moves
    public float minStrengthToWrite = 0.15f;    // don’t write super-weak observations
    public float fadeExponent = 2f;

    [Header("Ghost Look")]
    public Material ghostMaterial;              // your white aura material (transparent/emissive)

    private class Record
    {
        public Memorable src;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public float lastSeenTime;
        public float confidence; // 1..0
        public float seed;
        public GhostInstance ghost;
    }

    private readonly Dictionary<string, Record> _mem = new Dictionary<string, Record>();

    void Awake()
    {
        Instance = this;
    }

    public void Observe(Memorable m, float strength , bool isFading = true)
    {
        if (!m || strength < minStrengthToWrite) return;

        if (!_mem.TryGetValue(m.Guid, out var rec))
        {
            rec = new Record
            {
                src = m,
                seed = Random.value * 1000f,
                confidence = 1f,
                lastSeenTime = Time.time,
            };
            _mem[m.Guid] = rec;
            
            rec.ghost = GhostInstance.Create(ghostMaterial, m , m.color , m.layer , isFading);
        }

        // Update last-known transform (this is the “truth” snapshot)
        rec.pos = m.transform.position;
        rec.rot = m.transform.rotation;
        rec.scale = m.transform.lossyScale;
        rec.lastSeenTime = Time.time;

        // Refresh confidence
        rec.confidence = Mathf.Clamp01(Mathf.Max(rec.confidence, strength));

        // If the ghost exists, snap it close to truth (optionally smooth)
        rec.ghost.SetBaseTransform(rec.pos, rec.rot, rec.scale);
    }

    void Update()
    {
        float now = Time.time;
        var dead = new List<string>();

        foreach (var kv in _mem)
        {
            var rec = kv.Value;

            float age = now - rec.lastSeenTime;
            float t = Mathf.Clamp01(age / Mathf.Max(0.001f, memoryLifetime));
            rec.confidence = 1f - t;

            if (rec.confidence <= 0.001f)
            {
                rec.ghost.Destroy();
                dead.Add(kv.Key);
                continue;
            }

            // Drift grows as confidence falls
            float driftAmp = driftMaxMeters * (1f - rec.confidence);

            // Smooth drift via Perlin
            float n1 = Mathf.PerlinNoise(rec.seed, now * driftSpeed);
            float n2 = Mathf.PerlinNoise(rec.seed + 31.7f, now * driftSpeed);
            float n3 = Mathf.PerlinNoise(rec.seed + 99.1f, now * driftSpeed);

            Vector3 drift = new Vector3(n1 - 0.5f, (n2 - 0.5f) * 0.3f, n3 - 0.5f) * (2f * driftAmp);

            // Continuous fade (starts immediately)
            float fade = Mathf.Pow(rec.confidence, fadeExponent);
            rec.ghost.UpdateGhost(drift, fade , Time.deltaTime);
        }

        foreach (var k in dead) _mem.Remove(k);
    }

}
