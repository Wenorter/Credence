using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuIntro : MonoBehaviour
{
    public Image fadeImage;
    public Camera cam;
    public float duration = 2.5f;
    public float startDelay = 0.3f;
    public float dollyDistance = 3f; // CHANGED: how far back the camera starts

    IEnumerator Start()
    {
        Transform t = cam.transform; // CHANGED
        Vector3 endPos = t.position; // CHANGED
        Vector3 startPos = endPos - t.forward * dollyDistance; // CHANGED

        t.position = startPos; // CHANGED
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = true;

        yield return new WaitForSecondsRealtime(startDelay);

        for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, time / duration);
            t.position = Vector3.Lerp(startPos, endPos, k); // CHANGED
            fadeImage.color = new Color(0, 0, 0, 1f - k);
            yield return null;
        }

        t.position = endPos; // CHANGED
        fadeImage.color = Color.clear;
        fadeImage.raycastTarget = false;
    }
}