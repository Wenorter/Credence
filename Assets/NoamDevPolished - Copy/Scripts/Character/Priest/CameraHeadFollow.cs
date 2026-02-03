using UnityEngine;

[DisallowMultipleComponent]
public class CameraHeadFollow : MonoBehaviour
{
    private enum FollowMode
    {
        HumanoidHead,
        ManualTransform
    }

    [Header("References")]
    [Tooltip("Character Animator that drives the skeleton.")]
    [SerializeField] private Animator animator;

    [Tooltip("How we find the head transform.")]
    [SerializeField] private FollowMode followMode = FollowMode.HumanoidHead;

    [Tooltip("Used when Follow Mode = Manual Transform.")]
    [SerializeField] private Transform headTransform;

    [Header("Offsets")]
    [Tooltip("Local offset relative to the head orientation. Use this to move the camera slightly forward/up.")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.02f, 0.08f);

    [Tooltip("If true, the pivot also copies head rotation. Usually OFF for FPS (you already handle look).")]
    [SerializeField] private bool followRotation;

    private Transform _resolvedHead;

    private void Reset()
    {
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    private void Awake()
    {
        if (animator == null)
        {
            Debug.LogError("CameraHeadFollow: animator is not set.", this);
            enabled = false;
            return;
        }

        ResolveHead();
        if (_resolvedHead == null)
        {
            Debug.LogError("CameraHeadFollow: could not resolve head transform.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (_resolvedHead == null)
            return;

        // Head bone position is in world space.
        var headPos = _resolvedHead.position;
        var headRot = _resolvedHead.rotation;

        transform.position = headPos + headRot * localOffset;

        if (followRotation)
            transform.rotation = headRot;
    }

    private void ResolveHead()
    {
        if (followMode == FollowMode.ManualTransform)
        {
            _resolvedHead = headTransform;
            return;
        }

        // Humanoid rig: this is the most reliable way.
        _resolvedHead = animator.GetBoneTransform(HumanBodyBones.Head);

        // Some rigs return null if not Humanoid.
        if (_resolvedHead == null)
            _resolvedHead = headTransform;
    }
}
