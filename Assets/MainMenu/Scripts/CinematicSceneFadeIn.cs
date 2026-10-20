using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CinematicFadeIn : MonoBehaviour
{
    public Material fadeMaterial;
    public float duration = 3f;
    public float startDelay = 0.3f;
    public bool playOnStart = true;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public UnityEvent onComplete;

    static readonly int ProgressId = Shader.PropertyToID("_Progress");
    Image img;
    Material mat;

    void Awake()
    {
        img = GetComponent<Image>();
        mat = new Material(fadeMaterial);
        img.material = mat;
        mat.SetFloat(ProgressId, 0f);
        img.raycastTarget = true;
    }

    void Start() { if (playOnStart) Play(); }

    public void Play()
    {
        StopAllCoroutines();
        img.enabled = true;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        mat.SetFloat(ProgressId, 0f);
        yield return new WaitForSecondsRealtime(startDelay);

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            mat.SetFloat(ProgressId, curve.Evaluate(t / duration));
            yield return null;
        }

        mat.SetFloat(ProgressId, 1f);
        img.raycastTarget = false;
        img.enabled = false;
        onComplete?.Invoke();
    }

    void OnDestroy() { if (mat) Destroy(mat); }
}