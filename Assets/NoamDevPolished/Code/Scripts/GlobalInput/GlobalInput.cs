// GlobalInput.cs
//
// What this script does:
// - Listens for charSwitchButton each frame.
// - When charSwitchButton is pressed: fades the screen to black over X seconds,
//   switches the active overview (Priest <-> Angel) + invokes an event,
//   then fades back to normal.
// - Prevents spamming charSwitchButton while the fade/switch is in progress.
// - Ensures the fade overlay GameObject is enabled at runtime (even if it was disabled in the scene).

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class GlobalInput : MonoBehaviour
{
    [Header("Overviews")]
    [Tooltip("Root object to enable/disable when controlling the Priest (camera rig, controller root, etc.).")]
    [SerializeField] private GameObject priestOverview;
    
    [Tooltip("Root object to enable/disable when controlling the Angel (camera rig, controller root, etc.).")]
    [SerializeField] private GameObject angelOverview;
    //
    
    [Header("Fade Overlay")]
    [Tooltip("CanvasGroup on a full-screen black UI Image. Alpha 0 = visible game, Alpha 1 = fully black.")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Min(0f)]
    [Tooltip("Seconds to fade from normal to black.")]
    [SerializeField] private float fadeToBlackSeconds = 1.25f;

    [Min(0f)]
    [Tooltip("Seconds to fade from black back to normal.")]
    [SerializeField] private float fadeFromBlackSeconds = 1.25f;
    //
    
    [Header("Events")]
    [Tooltip("Invoked while the screen is black, right before the overviews are toggled.")]
    [SerializeField] private UnityEvent onSwitchCharEvent = new();
    //
    
    private bool _isSwitching;

    private void Awake()
    {
        // If the fade overlay object was disabled in the scene, enable it so we can control alpha reliably.
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning($"{nameof(GlobalInput)}: Fade CanvasGroup is not assigned. Switching will work, but there will be no fade.", this);
        }
        else
        {
            var fadeGo = fadeCanvasGroup.gameObject;
            if (!fadeGo.activeSelf)
                fadeGo.SetActive(true);

            // Start with a clear screen and make sure the overlay doesn't block clicks during gameplay.
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        if (priestOverview == null || angelOverview == null)
            Debug.LogWarning($"{nameof(GlobalInput)}: One or both overview references are missing. Switching may not behave as expected.", this);
    }

    private void OnValidate()
    {
        if (fadeToBlackSeconds < 0f) fadeToBlackSeconds = 0f;
        if (fadeFromBlackSeconds < 0f) fadeFromBlackSeconds = 0f;
    }

    private void Update()
    {
        if (_isSwitching)
            return;

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.tabKey.wasPressedThisFrame)
            StartCoroutine(SwitchWithFade());
    }

    private IEnumerator SwitchWithFade()
    {
        _isSwitching = true;

        // Fade to black (if we have a fade overlay).
        if (fadeCanvasGroup != null)
        {
            if (fadeToBlackSeconds > 0f)
                yield return FadeCanvasGroup(fadeCanvasGroup, targetAlpha: 1f, duration: fadeToBlackSeconds);

            // Lock in full black before switching, and block UI clicks during the transition.
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = true;
        }

        // Switch while black.
        onSwitchCharEvent.Invoke();

        if (priestOverview != null)
            priestOverview.SetActive(!priestOverview.activeSelf);
        else
            Debug.LogWarning($"{nameof(GlobalInput)}: Priest overview is not assigned.", this);

        if (angelOverview != null)
            angelOverview.SetActive(!angelOverview.activeSelf);
        else
            Debug.LogWarning($"{nameof(GlobalInput)}: Angel overview is not assigned.", this);

        // Fade back to normal.
        if (fadeCanvasGroup != null)
        {
            if (fadeFromBlackSeconds > 0f)
                yield return FadeCanvasGroup(fadeCanvasGroup, targetAlpha: 0f, duration: fadeFromBlackSeconds);

            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        _isSwitching = false;
    }

    private static IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
    {
        var startAlpha = group.alpha;

        if (Mathf.Approximately(duration, 0f))
        {
            group.alpha = targetAlpha;
            yield break;
        }

        var t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            var lerp = Mathf.Clamp01(t / duration);
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);

            yield return null;
        }

        group.alpha = targetAlpha;
    }
}
