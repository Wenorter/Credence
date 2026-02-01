using System.Collections.Generic;
using UnityEngine;

public class SenseProbe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FpsMovement fpsMovement;
    [SerializeField] private Camera visionCameraOverride;

    [Header("Sense Distance (per tag)")]
    [Tooltip("Default max sense distance if tag not found in list.")]
    public float defaultRadius = 4.0f;

    [Tooltip("Optional global cap (0 = no cap). If set > 0, physics query uses this max for performance.")]
    [SerializeField] private float globalMaxRadiusCap = 0f;

    [Tooltip("Per-tag max distance to sense (tag dropdown).")]
    [SerializeField] private List<TagValPair> tagSeenDistances = new List<TagValPair>();

    [Header("Vision (Looking)")]
    [Range(1f, 179f)]
    [SerializeField] private float viewAngle = 90f;

    [Tooltip("If within this distance, we refresh memory even if not in view cone (still requires LOS).")]
    [SerializeField] private float alwaysRememberDistance = 1.5f;

    [Header("LOS Targeting")]
    [Tooltip("How many points on the target bounds to test for LOS. More = more robust, slightly more raycasts.")]
    [Range(1, 5)]
    [SerializeField] private int losSampleCount = 3;

    [Tooltip("If true, LOS aims at center/chest/head points on bounds (fixes plank gaps issue).")]
    [SerializeField] private bool useBoundsAimPoints = true;

    [Tooltip("Layers that block vision (walls/props/floor).")]
    [SerializeField] private LayerMask occluderMask;

    [Header("Physics")]
    [Tooltip("Layers that contain detectable targets (your 'Sensible' layer etc.).")]
    [SerializeField] private LayerMask senseMask;

    [Header("Update")]
    [SerializeField] private float tickRate = 30f;

    [Header("Debug")]
    [SerializeField] private bool debugDrawRays = false;

    private readonly Collider[] _hits = new Collider[256];

    private readonly HashSet<int> _seenRendererThisTick = new HashSet<int>();
    private readonly Dictionary<int, Renderer> _activeRenderers = new Dictionary<int, Renderer>();

    private readonly Dictionary<string, float> _tagDistanceLookup = new Dictionary<string, float>();

    private float _nextTick;
    private MaterialPropertyBlock _mpb;
    private float _cosHalfFov;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        if (!fpsMovement) fpsMovement = GetComponentInParent<FpsMovement>();

        RebuildTagLookup();
        RecomputeFovCache();
    }

    private void OnValidate()
    {
        RebuildTagLookup();
        RecomputeFovCache();
        losSampleCount = Mathf.Clamp(losSampleCount, 1, 5);
    }

    private void RecomputeFovCache()
    {
        _cosHalfFov = Mathf.Cos((viewAngle * 0.5f) * Mathf.Deg2Rad);
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
        float max = defaultRadius;

        for (int i = 0; i < tagSeenDistances.Count; i++)
            max = Mathf.Max(max, tagSeenDistances[i].value);

        max = Mathf.Max(max, alwaysRememberDistance);

        if (globalMaxRadiusCap > 0f)
            max = Mathf.Min(max, globalMaxRadiusCap);

        return Mathf.Max(0f, max);
    }

    private Camera GetVisionCamera()
    {
        if (visionCameraOverride != null) return visionCameraOverride;

        if (fpsMovement != null)
        {
            var cam = fpsMovement.GetComponentInChildren<Camera>();
            if (cam != null) return cam;
        }

        return Camera.main;
    }

    private static bool IsLayerInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private static Memorable FindMemorableRoot(Collider col)
    {
        if (!col) return null;

        var mem = col.GetComponentInParent<Memorable>();
        if (mem != null) return mem;

        Transform root = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform.root;

        mem = root.GetComponentInChildren<Memorable>(true);
        if (mem != null) return mem;

        mem = root.GetComponentInParent<Memorable>();
        return mem;
    }

    private Transform GetHighlightGroupRoot(Collider col, Memorable mem)
    {
        // Prefer Memorable root so "head collider" still highlights whole monster
        if (mem != null) return mem.transform;

        // Fallback: parent group
        return (col != null && col.transform.parent != null) ? col.transform.parent : (col != null ? col.transform : null);
    }

    private void ApplySenseToGroup(Transform groupRoot, float strength)
    {
        if (!groupRoot) return;

        var renderers = groupRoot.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var rend = renderers[i];
            if (!rend) continue;

            if (!IsLayerInMask(rend.gameObject.layer, senseMask))
                continue;

            int id = rend.GetInstanceID();
            _seenRendererThisTick.Add(id);
            _activeRenderers[id] = rend;

            rend.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_Sense", strength);
            _mpb.SetFloat("_IsMemory", 0f);
            rend.SetPropertyBlock(_mpb);
        }
    }

    private void ClearSense(Renderer rend)
    {
        if (!rend) return;

        rend.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Sense", 0f);
        _mpb.SetFloat("_IsMemory", 0f);
        rend.SetPropertyBlock(_mpb);
    }

    private static Bounds GetGroupBounds(Collider col, Transform groupRoot)
    {
        // Prefer renderer bounds (more representative than collider closest-point)
        if (groupRoot != null)
        {
            var renderers = groupRoot.GetComponentsInChildren<Renderer>(true);
            if (renderers != null && renderers.Length > 0 && renderers[0] != null)
            {
                Bounds b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    if (renderers[i] != null) b.Encapsulate(renderers[i].bounds);
                }
                return b;
            }
        }

        // Fallback to collider bounds
        return col.bounds;
    }

    private bool HasLineOfSightToTarget(
        Vector3 origin,
        Transform targetRoot,
        Bounds b,
        int rayMask,
        Color debugColor)
    {
        // sample heights from center -> upper -> top
        // 1 point: center
        // 2 points: center, upper
        // 3 points: center, upper, top
        // 4/5 points: add a couple more uppers
        Vector3 center = b.center;
        float ey = b.extents.y;

        // If bounds is flat, just use center
        if (ey < 0.0001f)
        {
            return RayHitsTarget(origin, center, targetRoot, rayMask, debugColor);
        }

        // Predefined normalized offsets (0=center, 1=top)
        // Ordered to reduce work: center first, then higher points
        float[] tVals = { 0f, 0.55f, 0.9f, 0.3f, 0.75f };

        int n = Mathf.Clamp(losSampleCount, 1, 5);

        for (int i = 0; i < n; i++)
        {
            float t = tVals[i];
            Vector3 p = center + Vector3.up * (t * ey);
            if (RayHitsTarget(origin, p, targetRoot, rayMask, debugColor))
                return true;
        }

        return false;
    }

    private bool RayHitsTarget(Vector3 origin, Vector3 targetPoint, Transform targetRoot, int rayMask, Color debugColor)
    {
        Vector3 to = targetPoint - origin;
        float dist = to.magnitude;
        if (dist < 0.0001f) return true;

        Vector3 dir = to / dist;

        // small forward offset to avoid starting inside a collider
        Vector3 rayStart = origin + dir * 0.05f;
        float rayDist = Mathf.Max(0f, dist - 0.05f);

        if (!Physics.Raycast(rayStart, dir, out RaycastHit hit, rayDist, rayMask, QueryTriggerInteraction.Ignore))
        {
            if (debugDrawRays) Debug.DrawRay(rayStart, dir * rayDist, Color.yellow, 1f / tickRate);
            return false;
        }

        bool ok = hit.collider.transform.IsChildOf(targetRoot);

        if (debugDrawRays)
            Debug.DrawRay(rayStart, dir * rayDist, ok ? debugColor : Color.red, 1f / tickRate);

        return ok;
    }

    private void Update()
    {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + 1f / Mathf.Max(1f, tickRate);

        _seenRendererThisTick.Clear();

        Camera cam = GetVisionCamera();
        if (cam == null) return;

        Vector3 origin = cam.transform.position;
        Vector3 forward = cam.transform.forward;

        float queryRadius = GetQueryRadius();

        int count = Physics.OverlapSphereNonAlloc(
            origin,
            queryRadius,
            _hits,
            senseMask,
            QueryTriggerInteraction.Ignore
        );

        // Ray must hit BOTH occluders and targets so the "first hit is target" test is correct.
        int rayMask = occluderMask | senseMask;

        for (int i = 0; i < count; i++)
        {
            Collider col = _hits[i];
            if (!col) continue;

            Memorable mem = FindMemorableRoot(col);
            Transform targetRoot = mem != null
                ? mem.transform
                : (col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform.root);

            string objTag = targetRoot.tag;
            float tagRadius = GetSenseDistanceForTag(objTag);
            if (tagRadius <= 0f) continue;

            // Distance for strength uses closest point (good)
            Vector3 closest = col.ClosestPoint(origin);
            float d = Vector3.Distance(origin, closest);

            bool withinAlways = d <= alwaysRememberDistance;

            if (!withinAlways && d > tagRadius)
                continue;

            // View cone only if not withinAlways
            if (!withinAlways)
            {
                Vector3 toClosest = closest - origin;
                float distToClosest = toClosest.magnitude;
                if (distToClosest < 0.0001f) continue;

                Vector3 dirToClosest = toClosest / distToClosest;
                float dot = Vector3.Dot(forward, dirToClosest);
                if (dot < _cosHalfFov)
                    continue;
            }

            // LOS uses bounds aim points (fixes plank gaps / aiming between boards)
            Transform groupRoot = GetHighlightGroupRoot(col, mem);
            Bounds b = useBoundsAimPoints ? GetGroupBounds(col, groupRoot) : col.bounds;

            bool hasLos = HasLineOfSightToTarget(
                origin,
                targetRoot,
                b,
                rayMask,
                Color.green
            );

            if (!hasLos) continue;

            float strength = withinAlways ? 1f : (1f - Mathf.Clamp01(d / Mathf.Max(0.001f, tagRadius)));

            // Group highlight: seeing any child highlights the whole Memorable root group (or parent fallback)
            ApplySenseToGroup(groupRoot, strength);

            // Refresh memory ONLY when truly visible
            if (mem != null && mem.ignoredProbeLayer != mem.layer)
                MemoryManager.Instance.Observe(mem, strength);
        }

        // Clear aura from things not sensed this tick
        var keys = new List<int>(_activeRenderers.Keys);
        foreach (int id in keys)
        {
            if (_seenRendererThisTick.Contains(id)) continue;
            ClearSense(_activeRenderers[id]);
            _activeRenderers.Remove(id);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Camera cam = GetVisionCamera();
        Vector3 p = cam ? cam.transform.position : transform.position;

        Gizmos.color = Color.white;
        float r = Application.isPlaying ? GetQueryRadius() : defaultRadius;
        Gizmos.DrawWireSphere(p, r);
    }
}
