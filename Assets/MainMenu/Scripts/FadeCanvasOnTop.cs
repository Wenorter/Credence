using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FadeCanvasOnTop : MonoBehaviour
{
    void Awake()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = short.MaxValue;
    }
}