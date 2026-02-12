using UnityEngine;

public class ScenePoint : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = new Color(0f, 0.35f, 0f, 1f);
    [SerializeField] private float gizmoRadius = 0.75f;

    [Header("Forward Ray (Z)")]
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private Color rayColor = Color.blue;

    [Header("Ray Start Offset")]
    [SerializeField] private float yOffset = 0.25f; // raises the start point

    private void OnDrawGizmos()
    {
        // Sphere
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);

        // Ray start point with Y offset (LOCAL Y)
        Vector3 start = transform.position + transform.up * yOffset;

        // Ray direction (LOCAL Z)
        Vector3 end = start + transform.forward * rayLength;

        Gizmos.color = rayColor;
        Gizmos.DrawLine(start, end);
    }
}