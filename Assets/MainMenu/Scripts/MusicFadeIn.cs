using System.Collections;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour // CHANGED
{
    public AudioSource source; // CHANGED
    public float duration = 2.5f; // CHANGED
    public float targetVolume = 1f; // CHANGED
    public float startDelay = 0.3f; // CHANGED

    IEnumerator Start() // CHANGED
    {
        source.volume = 0f; // CHANGED
        source.Play(); // CHANGED

        yield return new WaitForSecondsRealtime(startDelay); // CHANGED

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) // CHANGED
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration); // CHANGED
            yield return null;
        }

        source.volume = targetVolume; // CHANGED
    }
}