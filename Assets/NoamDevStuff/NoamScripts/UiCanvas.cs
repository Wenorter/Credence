using System.Collections;
using TMPro;
using UnityEngine;

public class UiCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textToFade;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 2f;

    private Coroutine _fadeRoutine;

    public void ChangeText(string value)
    {
        if (textToFade == null) return;

        textToFade.text = value;

        // Usually you want new text visible immediately
        SetAlpha(1f);
    }

    public void StartFadeOut()
    {
        if (textToFade == null) return;

        // Stop any existing fade and clear the handle
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        // IMPORTANT: always start fading from visible
        SetAlpha(1f);

        _fadeRoutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float duration = Mathf.Max(0.0001f, fadeOutDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float a = Mathf.Lerp(1f, 0f, Mathf.Clamp01(t));
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(0f);
        _fadeRoutine = null;
    }

    private void SetAlpha(float a)
    {
        var c = textToFade.color;
        c.a = a;
        textToFade.color = c;
    }
}