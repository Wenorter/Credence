using UnityEngine;

public class MenuParralax : MonoBehaviour
{
    public Camera cam;
    public float offSetMultiplier = 1f;
    public float smoothTime = 0.3f;
    
    private Vector2 startPosition;
    private Vector2 velocity;  // Changed to Vector2
    private RectTransform rectTransform;
    
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        Vector2 offset = cam.ScreenToViewportPoint(Input.mousePosition);
        
        // Calculate target position
        Vector2 targetPosition = startPosition + (offset * offSetMultiplier);
        
        // Smoothly move to target using anchoredPosition
        rectTransform.anchoredPosition = Vector2.SmoothDamp(
            rectTransform.anchoredPosition, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );
    }
}