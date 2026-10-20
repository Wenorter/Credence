using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button newGameButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("New game")]
    public string gameSceneName = "Game";
    public Image fadeOverlay;
    public float fadeOutDuration = 1f;

    bool loading;

    void Awake()
    {
        Time.timeScale = 1f;
        ClosePanels();
    }

    void OnEnable()
    {
        newGameButton.onClick.AddListener(NewGame);
        settingsButton.onClick.AddListener(OpenSettings);
        creditsButton.onClick.AddListener(OpenCredits);
        exitButton.onClick.AddListener(ExitGame);
    }

    void OnDisable()
    {
        newGameButton.onClick.RemoveListener(NewGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        creditsButton.onClick.RemoveListener(OpenCredits);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ClosePanels();
    }

    public void NewGame()
    {
        if (loading) return;
        loading = true;
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.enabled = true;
            fadeOverlay.raycastTarget = true;
            for (float t = 0; t < fadeOutDuration; t += Time.unscaledDeltaTime)
            {
                fadeOverlay.color = new Color(0, 0, 0, t / fadeOutDuration);
                yield return null;
            }
            fadeOverlay.color = Color.black;
        }

        yield return SceneManager.LoadSceneAsync(gameSceneName);
    }

    public void OpenSettings()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}