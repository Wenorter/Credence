using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class StayInsideBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    //
    public BoxCollider cage;

    private void LateUpdate()
    {
        var b = cage.bounds;

        var pos = transform.position;

        var clamped = new Vector3(
            Mathf.Clamp(pos.x, b.min.x, b.max.x),
            Mathf.Clamp(pos.y, b.min.y, b.max.y),
            Mathf.Clamp(pos.z, b.min.z, b.max.z)
        );

        var correction = clamped - pos;

        // Push back inside using CharacterController (safe, no teleport)
        if (correction.sqrMagnitude > 0f)
        {
            controller.Move(correction);
        }
    }
}