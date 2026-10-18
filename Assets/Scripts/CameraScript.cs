using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -5f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.5f, 2f);

    private float _pitch;

    public void SetPitch(float pitch)
    {
        _pitch = pitch;
    }

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

        Quaternion baseRotation = target.rotation;

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            baseRotation = Quaternion.LookRotation(
                lookDirection,
                Vector3.up
            );
        }

        Quaternion rotation =
            baseRotation * Quaternion.Euler(_pitch, 0f, 0f);

        transform.SetPositionAndRotation(position, rotation);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 tip = origin + forward * 2f;

        Gizmos.DrawLine(origin, tip);

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