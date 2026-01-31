using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera priestCamera;
    [SerializeField] private Camera angelCamera;
    [SerializeField] private GameObject priestInput;
    [SerializeField] private GameObject angelInput;
    [SerializeField] private GameObject angel;
    [SerializeField] private GameObject priest;
    [SerializeField] private BoxCollider startingCage;
    [SerializeField] private Transform priestSpawnPoint;

    [Header("Priest")]
    [SerializeField] private string priestCurrentRoom = "";
    [SerializeField] private int priestCurrHp = 5;
    [SerializeField] private int priestStartHp = 5;

    [Header("Angel")]
    [SerializeField] private int angelCurrentFear;
    [SerializeField] private int angelMaxFear = 5;
    [Tooltip("When angel fear reaches max, his fear resets and he gets locked out of moving/looking")]
    [SerializeField] private float angelStunDuration = 3f;
    [Tooltip("How strong the red overlay is while stunned.")]
    [Range(0f, 1f)]
    [SerializeField] private float stunTintAlpha = 0.35f;

    [Header("Camera Fade Switch")]
    [SerializeField] private float fadeOutTime = 0.25f;
    [SerializeField] private float blackHoldTime = 0.05f;
    [SerializeField] private float fadeInTime = 0.25f;

    [Header("User Interface")]
    [SerializeField] private UiCanvas uiCanvas;

    [Header("Hold R To Reset")]
    [SerializeField] private float desiredHoldTime = 2f;

    [Header("Events")]
    [SerializeField] private UnityEvent onResetDayMemoryMan;
    [SerializeField] private UnityEvent onResetDayAiMan;
    [SerializeField] private UnityEvent onResetDayObjMan;

    // fields
    private int _cameraIndex;
    private Camera[] _cameras;
    private GameObject[] _inputObj;

    private bool _isSwitching;
    private Coroutine _switchRoutine;

    private bool _isAngelStunned;
    private Coroutine _angelStunRoutine;

    // day system
    private int _currentDay = 1;
    private readonly int _maxDay = 3;

    // Overlay UI
    private Canvas _overlayCanvas;
    private Image _fadeImage;      // black fade overlay
    private Image _stunTintImage;  // red tint overlay

    // Hold-to-reset state
    private float _heldTime;
    private bool _ignoreResetHoldUntilRelease;

    private void Awake()
    {
        _cameras = new[] { priestCamera, angelCamera };
        _inputObj = new[] { priestInput, angelInput };

        CreateOverlays();
        SetFadeAlpha(0f);
        UpdateStunTint();
    }

    private void Start()
    {
        TrySwitchRoom("room1", startingCage);

        // Ensure only current camera/input are active at start
        for (int i = 0; i < _cameras.Length; i++)
        {
            bool active = (i == _cameraIndex);
            _cameras[i].gameObject.SetActive(active);
            _inputObj[i].SetActive(active);
        }

        uiCanvas.ChangeText($"Day {_currentDay}");

        // Requirement: start with die alpha = 0 (progress 0)
        _heldTime = 0f;
        _ignoreResetHoldUntilRelease = false;
        uiCanvas.SetDieTextAlpha(0f);

        ResetDay();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            OnSwitchCamera();

        // Keep instant reset on death
        if (priestCurrHp <= 0)
        {
            _ignoreResetHoldUntilRelease = false;
            ResetHoldState();
            ResetDay();
            return;
        }

        HandleHoldReset();
    }

    private void HandleHoldReset()
    {
        float holdTarget = Mathf.Max(0.0001f, desiredHoldTime);

        // After a reset, ignore holding R until the player releases it once.
        if (_ignoreResetHoldUntilRelease)
        {
            if (Keyboard.current.rKey.isPressed)
            {
                uiCanvas.SetDieTextAlpha(0f);
                return;
            }

            // Released -> re-arm
            _ignoreResetHoldUntilRelease = false;
            ResetHoldState();
            return;
        }

        // Not pressing R -> reset timer + alpha
        if (!Keyboard.current.rKey.isPressed)
        {
            ResetHoldState();
            return;
        }

        // Holding -> accumulate and update progress (0..1)
        _heldTime += Time.deltaTime;
        float t = Mathf.Clamp01(_heldTime / holdTarget);

        uiCanvas.SetDieTextAlpha(t);

        // Reached hold duration -> trigger reset once, then ignore until release
        if (t >= 1f)
        {
            uiCanvas.SetDieTextAlpha(1f); // UiCanvas caps to 170/255 (your change)

            _ignoreResetHoldUntilRelease = true;
            ResetDay();

            // While still holding R, the text should not stay visible
            uiCanvas.SetDieTextAlpha(0f);
            _heldTime = 0f;
        }
    }

    private void ResetHoldState()
    {
        _heldTime = 0f;
        if (uiCanvas != null)
            uiCanvas.SetDieTextAlpha(0f);
    }

    public void OnSwitchCamera()
    {
        if (_isSwitching) return;

        if (_switchRoutine != null)
            StopCoroutine(_switchRoutine);

        _switchRoutine = StartCoroutine(SwitchCameraFadeRoutine());
    }

    private IEnumerator SwitchCameraFadeRoutine()
    {
        _isSwitching = true;

        // Disable input during switch so nothing happens mid-fade
        for (int i = 0; i < _inputObj.Length; i++)
            _inputObj[i].SetActive(false);

        // Fade to black
        yield return Fade(0f, 1f, fadeOutTime);

        if (blackHoldTime > 0f)
            yield return new WaitForSeconds(blackHoldTime);

        // Switch cameras while fully black
        _cameras[_cameraIndex].gameObject.SetActive(false);
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
        _cameras[_cameraIndex].gameObject.SetActive(true);

        // Enable input ONLY if not stunned (angel is index 1)
        if (_cameraIndex == 1 && _isAngelStunned)
            angelInput.SetActive(false);
        else
            _inputObj[_cameraIndex].SetActive(true);

        // Update stun tint visibility after switching cameras
        UpdateStunTint();

        // Fade back in
        yield return Fade(1f, 0f, fadeInTime);

        _isSwitching = false;
        _switchRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        duration = Mathf.Max(0.0001f, duration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t));
            SetFadeAlpha(a);
            yield return null;
        }

        SetFadeAlpha(to);
    }

    private void CreateOverlays()
    {
        // One canvas for both overlays, always on top
        var canvasGO = new GameObject("OverlayCanvas");
        _overlayCanvas = canvasGO.AddComponent<Canvas>();
        _overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _overlayCanvas.sortingOrder = short.MaxValue;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        DontDestroyOnLoad(canvasGO);

        // Stun tint (red) - should sit UNDER the black fade (so fade can fully black out)
        _stunTintImage = CreateFullScreenImage(_overlayCanvas.transform, "StunTintImage", new Color(1f, 0f, 0f, 0f));
        _stunTintImage.raycastTarget = false;

        // Fade (black) - top layer
        _fadeImage = CreateFullScreenImage(_overlayCanvas.transform, "FadeImage", new Color(0f, 0f, 0f, 0f));
        _fadeImage.raycastTarget = false;
    }

    private static Image CreateFullScreenImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = color;

        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return img;
    }

    private void SetFadeAlpha(float a)
    {
        if (!_fadeImage) return;
        var c = _fadeImage.color;
        c.a = Mathf.Clamp01(a);
        _fadeImage.color = c;
    }

    private void UpdateStunTint()
    {
        if (!_stunTintImage) return;

        // Show red tint ONLY when:
        // - angel is stunned
        // - and we are currently on angel camera (index 1)
        float targetAlpha = (_isAngelStunned && _cameraIndex == 1) ? stunTintAlpha : 0f;

        var c = _stunTintImage.color;
        c.a = Mathf.Clamp01(targetAlpha);
        _stunTintImage.color = c;
    }

    public void TrySwitchRoom(string newRoom, BoxCollider newRoomCage)
    {
        priestCurrentRoom = newRoom;

        angel.GetComponent<StayInsideBox>().cage = newRoomCage;

        var cc = angel.GetComponent<CharacterController>();
        cc.enabled = false;
        angel.transform.position = newRoomCage.bounds.center;
        cc.enabled = true;

        DebugUI.OnChangeText(TextContext.CurrentRoomText, priestCurrentRoom);
    }

    public void OnPriestAttacked()
    {
        priestCurrHp--;
        DebugUI.OnChangeText(TextContext.PriestHp, priestCurrHp.ToString());
        AngelSpooked();
    }

    private void AngelSpooked()
    {
        angelCurrentFear++;
        DebugUI.OnChangeText(TextContext.AngelSpooked, angelCurrentFear.ToString());

        if (angelCurrentFear >= angelMaxFear)
        {
            DebugUI.OnChangeText(TextContext.AngelStunned, "STUNNED");
            angelCurrentFear = 0;
            StartAngelStun();
        }
    }

    private void StartAngelStun()
    {
        _isAngelStunned = true;

        // If we're currently on the angel camera (index 1), disable its input immediately
        if (_cameraIndex == 1)
            angelInput.SetActive(false);

        UpdateStunTint();

        if (_angelStunRoutine != null)
            StopCoroutine(_angelStunRoutine);

        _angelStunRoutine = StartCoroutine(AngelStunRoutine());
    }

    private IEnumerator AngelStunRoutine()
    {
        yield return new WaitForSeconds(angelStunDuration);

        _isAngelStunned = false;

        // Re-enable input ONLY if we're currently on angel camera (index 1)
        if (_cameraIndex == 1)
            angelInput.SetActive(true);

        UpdateStunTint();

        _angelStunRoutine = null;
    }

    private void ResetDay()
    {
        if (!_cameras[0].isActiveAndEnabled)
        {
            OnSwitchCamera();
        }

        TrySwitchRoom("room1", startingCage);
        onResetDayMemoryMan.Invoke();
        onResetDayAiMan.Invoke();
        onResetDayObjMan.Invoke();

        priestCurrHp = priestStartHp;

        var cc = priest.GetComponent<CharacterController>();
        cc.enabled = false;
        priest.transform.position = priestSpawnPoint.position;
        var fpsM = priest.GetComponent<FpsMovement>();
        fpsM.LockViewTo(priestSpawnPoint);
        cc.enabled = true;
        
        uiCanvas.StartFadeOut();
    }

    private void CompleteCurrentDay()
    {
        if (_currentDay >= _maxDay)
        {
            // finish game.
        }
        else
        {
            _currentDay++;
            uiCanvas.ChangeText($"Day {_currentDay}");
        }
    }
}
