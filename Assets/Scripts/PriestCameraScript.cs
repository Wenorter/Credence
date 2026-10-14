using UnityEditor;
using UnityEngine;

public class PriestCameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -5f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.5f, 2f);
    
    private void LateUpdate()
    {
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        if (!target)
            return;

        Vector3 position =
            target.position + target.rotation * offset;

        Vector3 lookPoint =
            target.position + target.rotation * lookOffset;

        Vector3 lookDirection = lookPoint - position;
        Quaternion rotation = transform.rotation;

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            rotation = Quaternion.LookRotation(
                lookDirection,
                Vector3.up
            );
        }

        transform.SetPositionAndRotation(position, rotation);
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 tip = origin + forward * 2f;

        // Arrow shaft
        Gizmos.DrawLine(origin, tip);

        // Arrow head
        Vector3 headBase = tip - forward * 0.3f;

        Gizmos.DrawLine(
            tip,
            headBase + transform.right * 0.3f * 0.5f
        );

        Gizmos.DrawLine(
            tip,
            headBase - transform.right * 0.3f * 0.5f
        );
    }
}