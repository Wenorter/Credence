using UnityEngine;
using UnityEngine.InputSystem;

public class Highlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform raycastStart;          // empty child on the angel
    [SerializeField] private float maxDistance = 6f;
    [SerializeField] private LayerMask hitMask = ~0;          // everything by default

    [Header("Highlight")]
    [SerializeField] private string objectiveTag = "Objective";
    [SerializeField] private Color highlightColor = new Color(1f, 0.82f, 0.2f, 1f); // golden

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        Debug.Log("Left Click");
        if (raycastStart == null)
            return;

        // Raycast forward from the raycastStart (aim this object to the middle of the screen)
        Ray ray = new Ray(raycastStart.position, raycastStart.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Only react to Objective tag
            if (hit.collider.CompareTag(objectiveTag))
            {
                // Add or get Memorable on the hit object
                Memorable memorable = hit.collider.GetComponent<Memorable>();
                if (memorable == null)
                    memorable = hit.collider.gameObject.AddComponent<Memorable>();

                // Set the highlight color on the Memorable component
                memorable.color = highlightColor;
                memorable.layer = "Objective";
                memorable.ignoredProbeLayer = "Objective";
                MemoryManager.Instance.Observe(memorable, 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (raycastStart == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(raycastStart.position, raycastStart.position + raycastStart.forward * maxDistance);
    }
}