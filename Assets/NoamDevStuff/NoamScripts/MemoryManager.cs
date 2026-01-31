using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct TagValPair
{
    [TagSelector] public string tag;
    public float value;
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
    public float driftMaxMeters = 1.2f;
    public float driftSpeed = 0.25f;
    public float minStrengthToWrite = 0.15f;
    public float fadeExponent = 2f;

    [Header("Drift Tuning (Subtle Memory Drift)")]
    [Tooltip("Overall drift intensity multiplier (smaller = more subtle).")]
    [SerializeField] private float driftStrengthMultiplier = 0.25f;

    [Header("Drift Ramp (Linear by fade-out progress)")]
    [Tooltip("Drift starts only after this fraction of fade-out progress (0 = immediately, 0.3 = after 30% faded).")]
    [Range(0f, 0.95f)]
    [SerializeField] private float driftStartAfterFadeFraction = 0.0f;

    [Header("Fade-In")]
    [Tooltip("How long it takes to fade IN from 0 to target when seen.")]
    [SerializeField] private float fadeInDurationSeconds = 3f;

    // NEW: fade-in curve shaping (inspector graph)
    [Tooltip("If enabled, fade-in alpha rise is shaped by this curve (x=0..1 progress, y=0..1).")]
    [SerializeField] private bool useFadeInCurve = true;

    [Tooltip("Fade-in shaping curve: x=progress (0..1), y=alpha multiplier (0..1). Steep early => fast rise.")]
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Tooltip("How long after Observe() we consider the object 'seen' (seconds). Set ~ 2/tickRate of your SenseProbe.")]
    [SerializeField] private float seenWindowSeconds = 0.1f;

    [Tooltip("Prevents 'invisible when seen' by forcing memory to at least this value when Observe() happens.")]
    [Range(0f, 1f)]
    [SerializeField] private float minVisibleOnSeen = 0.15f;

    [Header("Forced Seen (Angel Highlight)")]
    [Tooltip("When Observe(..., isFading=false) is used (Angel highlight), keep it 'seen' for this long so it can fade in without needing many clicks.")]
    [SerializeField] private float forceSeenDurationSeconds = 3f;

    [Header("Drift Return When Seen")]
    [Tooltip("How quickly the drift offset returns to 0 when the object is seen again (higher = snaps faster).")]
    [SerializeField] private float driftReturnSpeed = 6f;

    [Header("Ghost Look")]
    public Material ghostMaterial;

    private class Record
    {
        public Memorable src;
        public string tag;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;

        public float lastSeenTime;
        public float confidence;
        public float lastSeenStrength;
        public float lastSeenStrengthRef;

        public float seed;
        public GhostInstance ghost;

        public float maxAlpha;
        public Vector3 driftOffset;

        public float forcedSeenUntil;
    }

    private readonly Dictionary<string, Record> _mem = new Dictionary<string, Record>();
    private readonly Dictionary<string, float> _tagLifetimeLookup = new Dictionary<string, float>();

    private void Awake()
    {
        Instance = this;
        RebuildTagLookup();
    }

    private void OnValidate()
    {
        RebuildTagLookup();
        forceSeenDurationSeconds = Mathf.Max(0f, forceSeenDurationSeconds);

        // NEW: keep curve in a sane state
        if (fadeInCurve == null || fadeInCurve.length == 0)
            fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
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
            kv.Value?.ghost?.Destroy();

        _mem.Clear();
    }

    public void Observe(Memorable m, float strength, bool isFading = true)
    {
        if (!m || strength < minStrengthToWrite) return;

        strength = Mathf.Clamp01(strength);

        if (!_mem.TryGetValue(m.Guid, out var rec))
        {
            rec = new Record
            {
                src = m,
                tag = m.gameObject.tag,
                seed = Random.value * 1000f,

                confidence = 0f,
                lastSeenTime = Time.time,
                lastSeenStrength = strength,
                lastSeenStrengthRef = strength,

                maxAlpha = Mathf.Clamp01(m.color.a),
                driftOffset = Vector3.zero,
                forcedSeenUntil = 0f,
            };
            _mem[m.Guid] = rec;

            rec.ghost = GhostInstance.Create(ghostMaterial, m, m.color, m.layer, isFading);
        }
        else
        {
            rec.tag = m.gameObject.tag;
            rec.maxAlpha = Mathf.Clamp01(m.color.a);

            rec.lastSeenTime = Time.time;
            rec.lastSeenStrength = Mathf.Max(rec.lastSeenStrength, strength);
            rec.lastSeenStrengthRef = Mathf.Max(rec.lastSeenStrengthRef, strength);
        }

        if (!isFading && forceSeenDurationSeconds > 0f)
            rec.forcedSeenUntil = Mathf.Max(rec.forcedSeenUntil, Time.time + forceSeenDurationSeconds);

        rec.pos = m.transform.position;
        rec.rot = m.transform.rotation;
        rec.scale = m.transform.lossyScale;
        rec.ghost.SetBaseTransform(rec.pos, rec.rot, rec.scale);

        rec.confidence = Mathf.Max(rec.confidence, minVisibleOnSeen);
    }

    private void Update()
    {
        float now = Time.time;
        var dead = new List<string>();

        foreach (var kv in _mem)
        {
            var rec = kv.Value;

            if (rec.src != null)
            {
                rec.pos = rec.src.transform.position;
                rec.rot = rec.src.transform.rotation;
                rec.scale = rec.src.transform.lossyScale;

                rec.maxAlpha = Mathf.Clamp01(rec.src.color.a);

                rec.ghost.SetBaseTransform(rec.pos, rec.rot, rec.scale);
            }

            float lifetime = GetLifetimeForTag(rec.tag);
            if (lifetime <= 0f)
            {
                rec.ghost.Destroy();
                dead.Add(kv.Key);
                continue;
            }

            bool seenRecently =
                (now - rec.lastSeenTime) <= Mathf.Max(0.01f, seenWindowSeconds)
                || now <= rec.forcedSeenUntil;

            float target = seenRecently ? rec.lastSeenStrength : 0f;

            float prev = rec.confidence;

            float fadeOutRate = Time.deltaTime / Mathf.Max(0.001f, lifetime);
            float fadeInRate  = Time.deltaTime / Mathf.Max(0.001f, fadeInDurationSeconds);

            float rate = (target > rec.confidence) ? fadeInRate : fadeOutRate;
            rec.confidence = Mathf.MoveTowards(rec.confidence, target, rate);

            if (!seenRecently)
                rec.lastSeenStrength = 0f;

            if (rec.confidence <= 0.001f)
            {
                rec.ghost.Destroy();
                dead.Add(kv.Key);
                continue;
            }

            // -----------------------------
            // NEW: curve-shaped fade-in alpha
            // -----------------------------
            float shapedConfidence = rec.confidence;

            bool fadingOut = rec.confidence < prev - 0.000001f;
            bool fadingIn  = rec.confidence > prev + 0.000001f;

            // Only shape when building up (fade-in). Fade-out remains your existing exponent/lifetime behavior.
            if (useFadeInCurve && !fadingOut)
            {
                // Normalize confidence progress across the fade-in range [0..target]
                float denom = Mathf.Max(0.0001f, target); // target is lastSeenStrength when seen
                float p = Mathf.Clamp01(shapedConfidence / denom); // 0..1 progress to target

                float c = Mathf.Clamp01(fadeInCurve.Evaluate(p));  // 0..1 from curve

                // Convert back into confidence space (0..target)
                shapedConfidence = c * denom;

                // Safety
                shapedConfidence = Mathf.Clamp(shapedConfidence, 0f, denom);
            }

            float baseFade = Mathf.Pow(shapedConfidence, fadeExponent);
            float fade = Mathf.Clamp01(baseFade * rec.maxAlpha);

            if (fadingOut)
            {
                float refStrength = Mathf.Max(0.001f, rec.lastSeenStrengthRef);
                float fadeOutT = 1f - Mathf.Clamp01(rec.confidence / refStrength);

                float driftT = Mathf.InverseLerp(driftStartAfterFadeFraction, 1f, fadeOutT);
                driftT = Mathf.Clamp01(driftT);

                float driftAmp =
                    driftMaxMeters *
                    Mathf.Clamp01(driftStrengthMultiplier) *
                    driftT;

                if (driftAmp > 0.00001f)
                {
                    float n1 = Mathf.PerlinNoise(rec.seed, now * driftSpeed);
                    float n2 = Mathf.PerlinNoise(rec.seed + 31.7f, now * driftSpeed);
                    float n3 = Mathf.PerlinNoise(rec.seed + 99.1f, now * driftSpeed);

                    Vector3 noise = new Vector3(n1 - 0.5f, (n2 - 0.5f) * 0.15f, n3 - 0.5f) * 2f;
                    Vector3 desiredDrift = noise * driftAmp;

                    rec.driftOffset = Vector3.Lerp(rec.driftOffset, desiredDrift, 4f * Time.deltaTime);
                }
            }
            else if (fadingIn || seenRecently)
            {
                rec.driftOffset = Vector3.Lerp(rec.driftOffset, Vector3.zero,
                    Mathf.Max(0.01f, driftReturnSpeed) * Time.deltaTime);

                if (seenRecently)
                    rec.lastSeenStrengthRef = Mathf.Max(0.001f, target);
            }

            rec.ghost.UpdateGhost(rec.driftOffset, fade, Time.deltaTime);
        }

        foreach (var k in dead)
            _mem.Remove(k);
    }

    public bool ApplyGhostFromMemorable(Memorable m, bool restartFadeIn = false)
    {
        if (m == null) return false;

        if (!_mem.TryGetValue(m.Guid, out var rec) || rec == null)
            return false;

        if (rec.ghost == null)
            return false;

        rec.ghost.ApplyFromMemorable(m, restartFadeIn, fadeInDurationSeconds);

        rec.tag = m.gameObject.tag;
        rec.maxAlpha = Mathf.Clamp01(m.color.a);

        rec.pos = m.transform.position;
        rec.rot = m.transform.rotation;
        rec.scale = m.transform.lossyScale;
        rec.ghost.SetBaseTransform(rec.pos, rec.rot, rec.scale);

        return true;
    }
}
