using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AngelActions : MonoBehaviour
{
    [Header("Aim / Raycast")]
    [Tooltip("MUST include pitch. Set this to your cameraPivot (the thing that rotates up/down).")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float maxDistance = 6f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Highlight")]
    [SerializeField] private string objectiveTag = "Objective";
    [SerializeField] private Color highlightColor = new(1f, 0.82f, 0.2f, 1f);

    [Header("Throwable Pickup / Throw")]
    [SerializeField] private string throwableTag = "Throwable";
    [SerializeField] private float holdDistance = 1.2f;

    [Tooltip("Offset in aimTransform local space. This is subtracted from the screen center (center - offset).")]
    [SerializeField] private Vector3 holdOffset = new(0.25f, -0.15f, 0f);

    [SerializeField] private float holdFollowSpeed = 18f;
    [SerializeField] private float holdRotateSpeed = 18f;
    [SerializeField] private float throwForce = 14f;

    [Header("Held Visuals (Material Swap)")]
    [Tooltip("Assign your transparent material template here (e.g. Unlit/Transparent).")]
    [SerializeField] private Material heldTransparentMaterial;

    [Range(0f, 1f)]
    [Tooltip("Alpha to use while held.")]
    [SerializeField] private float heldAlpha = 0.35f;

    [Tooltip("Apply transparency to renderers on the object and its children.")]
    [SerializeField] private bool affectChildRenderers = true;

    private Rigidbody _heldRb;
    private Collider _heldCol;

    // Restore original materials + per-held instance
    private readonly List<Renderer> _heldRenderers = new();
    private readonly List<Material[]> _originalMaterials = new();
    private Material _heldMatInstance;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // If holding: throw
        if (_heldRb != null)
        {
            ThrowHeld();
            return;
        }

        // Otherwise: highlight objective
        if (aimTransform == null) return;

        var ray = new Ray(aimTransform.position, aimTransform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag(objectiveTag))
            {
                var memorable = hit.collider.gameObject.GetComponent<Memorable>();

                memorable.color = highlightColor;
                memorable.layer = "Objective";
                memorable.tag = "Objective";
                memorable.ignoredProbeLayer = "Default";
                memorable.isTrigger = true;
                Debug.Log("Hit! !! 111 1 1");
                MemoryManager.Instance.Observe(memorable, 1, false);    
                MemoryManager.Instance.TryApplyGhostFromMemorable(memorable);
            }
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_heldRb != null) return;
        if (aimTransform == null) return;

        var ray = new Ray(aimTransform.position, aimTransform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag(throwableTag))
                TryPickUp(hit.collider);
        }
    }
    
    

    private void TryPickUp(Collider hitCol)
    {
        var rb = hitCol.attachedRigidbody;
        if (rb == null) return;

        _heldRb = rb;
        _heldCol = hitCol;

        // Disable collision so it won't bump while being pulled/held
        _heldCol.enabled = false;

        // No physics while held
        _heldRb.useGravity = false;
        _heldRb.isKinematic = true;
        _heldRb.linearVelocity = Vector3.zero;
        _heldRb.angularVelocity = Vector3.zero;

        ApplyHeldTransparentMaterial(_heldRb.gameObject);
    }

    // Use LateUpdate so it runs AFTER your FpsMovement rotates yaw/pitch in Update.
    private void LateUpdate()
    {
        if (_heldRb == null) return;
        if (aimTransform == null) return;

        Vector3 basePos = aimTransform.position + aimTransform.forward * holdDistance;

        Vector3 offsetWorld =
            aimTransform.right * holdOffset.x +
            aimTransform.up * holdOffset.y +
            aimTransform.forward * holdOffset.z;

        Vector3 targetPos = basePos - offsetWorld;
        Quaternion targetRot = aimTransform.rotation;

        float posT = 1f - Mathf.Exp(-holdFollowSpeed * Time.deltaTime);
        float rotT = 1f - Mathf.Exp(-holdRotateSpeed * Time.deltaTime);

        _heldRb.MovePosition(Vector3.Lerp(_heldRb.position, targetPos, posT));
        _heldRb.MoveRotation(Quaternion.Slerp(_heldRb.rotation, targetRot, rotT));
    }

    private void ThrowHeld()
    {
        if (_heldRb == null) return;

        // Restore original visuals first
        RestoreOriginalMaterials();

        // Re-enable collider + physics
        if (_heldCol != null)
            _heldCol.enabled = true;

        _heldRb.isKinematic = false;
        _heldRb.useGravity = true;

        Vector3 dir = aimTransform != null ? aimTransform.forward : transform.forward;
        _heldRb.AddForce(dir * throwForce, ForceMode.VelocityChange);

        _heldRb = null;
        _heldCol = null;
    }

    private void ApplyHeldTransparentMaterial(GameObject root)
    {
        RestoreOriginalMaterials(); // safety

        if (heldTransparentMaterial == null)
        {
            Debug.LogWarning($"{name}: heldTransparentMaterial is not assigned.");
            return;
        }

        // Create a per-held-object material instance (so we don't modify the asset)
        _heldMatInstance = new Material(heldTransparentMaterial);
        SetMaterialAlphaIfPossible(_heldMatInstance, heldAlpha);

        Renderer[] renderers = affectChildRenderers
            ? root.GetComponentsInChildren<Renderer>(true)
            : root.GetComponents<Renderer>();

        foreach (var r in renderers)
        {
            if (r == null) continue;

            _heldRenderers.Add(r);
            _originalMaterials.Add(r.sharedMaterials);

            // Replace every submesh material slot with the transparent instance
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
                mats[i] = _heldMatInstance;

            r.sharedMaterials = mats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < _heldRenderers.Count; i++)
        {
            var r = _heldRenderers[i];
            if (r == null) continue;
            r.sharedMaterials = _originalMaterials[i];
        }

        _heldRenderers.Clear();
        _originalMaterials.Clear();

        if (_heldMatInstance != null)
        {
            Destroy(_heldMatInstance);
            _heldMatInstance = null;
        }
    }

    private static void SetMaterialAlphaIfPossible(Material mat, float alpha)
    {
        if (mat == null) return;

        if (mat.HasProperty(BaseColorId))
        {
            var c = mat.GetColor(BaseColorId);
            c.a = alpha;
            mat.SetColor(BaseColorId, c);
        }
        else if (mat.HasProperty(ColorId))
        {
            var c = mat.GetColor(ColorId);
            c.a = alpha;
            mat.SetColor(ColorId, c);
        }
    }

    private void OnDisable()
    {
        // Safety: if this component gets disabled while holding something, restore visuals
        RestoreOriginalMaterials();
    }

    private void OnDrawGizmos()
    {
        if (aimTransform == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(aimTransform.position, aimTransform.position + aimTransform.forward * maxDistance);

        Vector3 basePos = aimTransform.position + aimTransform.forward * holdDistance;
        Vector3 offsetWorld =
            aimTransform.right * holdOffset.x +
            aimTransform.up * holdOffset.y +
            aimTransform.forward * holdOffset.z;

        Vector3 holdPos = basePos - offsetWorld;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(holdPos, 0.08f);
        Gizmos.DrawLine(holdPos, holdPos + aimTransform.forward * 0.35f);
    }
}
