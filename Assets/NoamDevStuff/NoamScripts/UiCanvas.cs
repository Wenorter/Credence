using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UiCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text dieText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 2f;
    [Range(0, 255)]
    [SerializeField] private int dieMaxAlpha255 = 170;

    private Coroutine _fadeRoutine;

    public void ChangeText(string value)
    {
        if (dayText == null) return;

        dayText.text = value;

        // Usually you want new text visible immediately
        SetAlpha(1f);
    }

    public void StartFadeOut()
    {
        if (dayText == null) return;

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
        var c = dayText.color;
        c.a = a;
        dayText.color = c;
    }
    // progress01: 0..1 from GameManager
    public void SetDieTextAlpha(float progress01)
    {
        if (!dieText) return;

        progress01 = Mathf.Clamp01(progress01);
        float maxA = dieMaxAlpha255 / 255f;   // 170/255
        float a = progress01 * maxA;          // 0..maxA

        var c = dieText.color;
        c.a = a;
        dieText.color = c;
    }
    
    
}