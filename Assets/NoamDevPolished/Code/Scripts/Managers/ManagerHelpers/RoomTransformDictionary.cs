// Assets/Scripts/Rooms/RoomTransformDictionary.cs
//
// RoomTransformDictionary
//
// What this script does:
// - Stores a dictionary-like mapping: RoomId -> Transform, editable in the Inspector.
// - Unity can't serialize Dictionary, so we serialize a List<Entry> instead.
// - At runtime we build a fast lookup Dictionary from the list.
// - When something isn't configured (missing key / missing transform), it can log a clear warning.
//
// Typical use:
// - Add [RoomDictionary] above a RoomTransformDictionary field on a MonoBehaviour.
// - In the Inspector: add entries, pick a RoomId, assign a Transform.
// - In code: roomPoints.TryGet(RoomId.StartingRoom, out var t) or roomPoints.GetOrLog(...)

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Room identifiers used as keys in RoomTransformDictionary.
/// Keep these stable once you start wiring scenes (renaming values breaks serialized data).
/// </summary>
public enum RoomId
{
    StartingRoom,
    Room2,
}

[Serializable]
public sealed class RoomTransformDictionary : ISerializationCallbackReceiver
{
    [Serializable]
    public sealed class Entry
    {
        [Tooltip("Which room this entry represents.")]
        public RoomId key;

        [Tooltip("Transform associated with this room (spawn point, center point, perch, etc.).")]
        public Transform value;
    }

    [Header("Room Map (Serialized)")]
    [Tooltip("Inspector-editable mapping. Keys should be unique. If duplicates exist, the last one wins.")]
    [SerializeField] private List<Entry> entries = new List<Entry>();

    [Header("Debug")]
    [Tooltip("If true, logs a warning when you request a key that isn't mapped.")]
    [SerializeField] private bool logMissingKeys = true;

    [Tooltip("If true, logs a warning when a mapped key has a null Transform.")]
    [SerializeField] private bool logNullValues = true;

    // Runtime-only lookup (not serialized).
    private readonly Dictionary<RoomId, Transform> _lookup = new Dictionary<RoomId, Transform>();
    private bool _isBuilt;

    public IReadOnlyList<Entry> Entries => entries;

    public void OnBeforeSerialize()
    {
        // Nothing required. 'entries' is the serialized source of truth.
    }

    public void OnAfterDeserialize()
    {
        // Rebuild lazily on first use. This avoids work during domain reload bursts.
        _isBuilt = false;
    }

    /// <summary>
    /// Returns true if the room key exists and its transform is not null.
    /// </summary>
    public bool TryGet(RoomId key, out Transform result)
    {
        EnsureBuilt();

        if (!_lookup.TryGetValue(key, out result))
        {
            return false;
        }

        return result != null;
    }

    /// <summary>
    /// Gets the transform for a room, or logs a warning and returns null.
    /// Pass a context object so clicking the log selects the right thing.
    /// </summary>
    public Transform GetOrLog(RoomId key, UnityEngine.Object logContext = null)
    {
        EnsureBuilt();

        if (!_lookup.TryGetValue(key, out var t))
        {
            if (logMissingKeys)
            {
                Debug.LogWarning(
                    $"RoomTransformDictionary: No entry found for '{key}'. Add it in the Inspector.",
                    logContext);
            }

            return null;
        }

        if (t == null)
        {
            if (logNullValues)
            {
                Debug.LogWarning(
                    $"RoomTransformDictionary: Entry for '{key}' exists but the Transform is not assigned.",
                    logContext);
            }

            return null;
        }

        return t;
    }

    /// <summary>
    /// Call this if you ever modify Entries at runtime (rare). Otherwise you can ignore it.
    /// </summary>
    public void MarkDirty()
    {
        _isBuilt = false;
    }

    private void EnsureBuilt()
    {
        if (_isBuilt)
            return;

        _lookup.Clear();

        for (var i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e == null)
                continue;

            // "Last one wins" keeps behavior deterministic if duplicates exist.
            _lookup[e.key] = e.value;
        }

        _isBuilt = true;
    }
}
