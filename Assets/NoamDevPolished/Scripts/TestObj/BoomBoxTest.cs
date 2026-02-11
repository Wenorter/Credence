// Assets/Scripts/Vision/BoomBoxTest.cs
//
// Test emitter for the vision sound system.
//
// Behavior:
// - Start a "sound" with a random duration (0.15..3.0 seconds)
// - While the sound is active, re-trigger the sound reveal every tickSeconds (Option A)
// - After it finishes, wait a random gap (0.5..5.0 seconds) and repeat
//
// Optional: play a real AudioClip at the start so you can hear when it begins.

using UnityEngine;

[DisallowMultipleComponent]
public sealed class BoomBoxTest : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private PriestVisionSoundRevealSystem soundReveal;

    [Header("Sound Duration (seconds)")]
    [Min(0.01f)]
    [SerializeField] private float minSoundSeconds = 0.15f;

    [Min(0.01f)]
    [SerializeField] private float maxSoundSeconds = 3.0f;

    [Header("Gap After Sound (seconds)")]
    [Min(0.01f)]
    [SerializeField] private float minGapSeconds = 0.5f;

    [Min(0.01f)]
    [SerializeField] private float maxGapSeconds = 5.0f;

    [Header("Trigger Tick (seconds)")]
    [Tooltip("How often we re-trigger while the sound is 'playing'.\nSmaller = steadier reveal but more calls. 0.10..0.25 is usually enough.")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float tickSeconds = 0.15f;

    [Header("Strength (0..1)")]
    [Range(0f, 1f)]
    [SerializeField] private float minStrength = 0.15f;

    [Range(0f, 1f)]
    [SerializeField] private float maxStrength = 1.0f;

    [Header("Optional Audio")]
    [Tooltip("If assigned, plays an audible cue at the start of each sound event.")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip clip;

    [Range(0f, 2f)]
    [SerializeField] private float volume = 1f;

    private float _soundEndTime;
    private float _nextTickTime;
    private float _nextStartTime;

    private float _currentStrength;
    private bool _isPlayingSound;

    private void OnValidate()
    {
        minSoundSeconds = Mathf.Max(0.01f, minSoundSeconds);
        maxSoundSeconds = Mathf.Max(0.01f, maxSoundSeconds);
        if (maxSoundSeconds < minSoundSeconds)
            maxSoundSeconds = minSoundSeconds;

        minGapSeconds = Mathf.Max(0.01f, minGapSeconds);
        maxGapSeconds = Mathf.Max(0.01f, maxGapSeconds);
        if (maxGapSeconds < minGapSeconds)
            maxGapSeconds = minGapSeconds;

        tickSeconds = Mathf.Clamp(tickSeconds, 0.05f, 0.5f);

        minStrength = Mathf.Clamp01(minStrength);
        maxStrength = Mathf.Clamp01(maxStrength);
        if (maxStrength < minStrength)
            maxStrength = minStrength;
    }

    private void OnEnable()
    {
        _isPlayingSound = false;
        _nextStartTime = Time.time; // start immediately
    }

    private void Update()
    {
        if (soundReveal == null)
            return;

        var now = Time.time;

        if (_isPlayingSound)
        {
            if (now >= _soundEndTime)
            {
                _isPlayingSound = false;
                ScheduleNextStart(now);
                return;
            }

            if (now >= _nextTickTime)
            {
                _nextTickTime = now + tickSeconds;
                soundReveal.TriggerSound(transform.position, _currentStrength);
            }

            return;
        }

        // Not currently playing a sound: wait until next start time.
        if (now < _nextStartTime)
            return;

        StartSound(now);
    }

    private void StartSound(float now)
    {
        _isPlayingSound = true;

        _currentStrength = Random.Range(minStrength, maxStrength);

        var dur = Random.Range(minSoundSeconds, maxSoundSeconds);
        _soundEndTime = now + dur;

        _nextTickTime = now; // trigger immediately

        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip, volume * _currentStrength);
    }

    private void ScheduleNextStart(float now)
    {
        _nextStartTime = now + Random.Range(minGapSeconds, maxGapSeconds);
    }
}
