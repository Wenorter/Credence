using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RoomManager : MonoBehaviour
{
    [Header("Room Transforms")]
    [RoomDictionary]
    [SerializeField] private RoomTransformDictionary rooms;

    [Header("Manager Specific Events")]
    [SerializeField] private UnityEvent<Transform> OnNotifyAngel = new();

    private RoomId _currentRoom;

    private int debugIndex;
    
    
    public void OnPriestCurrentRoomChanged(RoomId newRoom)
    {
        debugIndex++;
        _currentRoom = newRoom;
        
        if (!rooms.TryGet(_currentRoom, out var targetTransform)) return;
        
        Debug.Log($"CurrentRoom:  {_currentRoom} + {debugIndex}");
        
        OnNotifyAngel.Invoke(targetTransform);
    }
}