using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class AngelZone : MonoBehaviour
{
    [Header("Reset Position Mechanics")]
    [SerializeField] private string priestTag = "Player Priest";
    [SerializeField] private Transform angel;
    [SerializeField] private Transform resetPoint;

    [Header("Gizmos")]
    [SerializeField] private string zoneName = "Angel Zone";
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private int fontSize = 16;
    [SerializeField] private float resetPositionSphereRadius = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(priestTag))
            return;

        if (!angel || !resetPoint)
            return;

        angel.position = resetPoint.position;
        // angel.LookAt(other.transform.position);
        // TODO: make angel look to player in update after reset position so it will always face player after moving to next room, we need to make it as flag in actual angel behaviour script
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(EditorStyles.label)
        {
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.yellow }
        };

        Handles.Label(
            transform.position + offset,
            zoneName,
            style);

        if (!resetPoint)
            return;

        Color previousColor = Gizmos.color;
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            resetPoint.position,
            resetPositionSphereRadius);

        Gizmos.DrawSphere(
            resetPoint.position,
            resetPositionSphereRadius / 10f);

        Gizmos.color = previousColor;
    }
#endif
}