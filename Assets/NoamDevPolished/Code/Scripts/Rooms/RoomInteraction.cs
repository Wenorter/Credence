// Assets/Scripts/Rooms/RoomInteraction.cs
//
// RoomInteraction
//
// What this script does:
// - Acts like a simple trigger-based room signal for the priest only.
// - When a collider with tag "Priest" enters the trigger, it invokes an event with 'enteredRoom'.
// - When a collider with tag "Priest" exits the trigger, it invokes an event with 'exitRoom'.
//
// How to use:
// - Put this on a GameObject that has a Collider set to "Is Trigger".
// - Make sure your priest root (or the collider that enters) is tagged "Priest".
// - Assign enteredRoom / exitRoom in the Inspector.
// - Hook OnRoomSignal to whatever should react (RoomManager, Angel system, etc).
//
// Notes:
// - If you have multiple colliders on the priest, consider putting the tag on the collider root
//   or adjusting the check to look up the hierarchy (easy to add later).

using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class RoomInteraction : MonoBehaviour
{
    [Header("Room Ids")]
    [Tooltip("Value passed to the event when the priest enters this trigger.")]
    [SerializeField] private RoomId enteredRoom = RoomId.StartingRoom;

    [Tooltip("Value passed to the event when the priest exits this trigger.")]
    [SerializeField] private RoomId exitRoom = RoomId.StartingRoom;

    [Header("Events")]
    [Tooltip("Called when the priest enters/exits. Enter passes 'enteredRoom', Exit passes 'exitRoom'.")]
    [SerializeField] private UnityEvent<RoomId> onRoomSignal;

    private const string PriestTag = "Priest";
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PriestTag))
            return;

        if (onRoomSignal == null)
        {
            Debug.LogWarning("RoomInteraction: onRoomSignal is not assigned.", this);
            return;
        }

        onRoomSignal.Invoke(enteredRoom);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PriestTag))
            return;

        if (onRoomSignal == null)
        {
            Debug.LogWarning("RoomInteraction: onRoomSignal is not assigned.", this);
            return;
        }

        onRoomSignal.Invoke(exitRoom);
    }
}
