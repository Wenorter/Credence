using System;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string enteredRoom;
    [SerializeField] private string exitedRoom;
    [SerializeField] private BoxCollider enteredRoomCage;
    [SerializeField] private BoxCollider exitedRoomCage;
    [SerializeField] private GameManager gameManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Priest"))
        {
            gameManager.TrySwitchRoom(enteredRoom , enteredRoomCage);    
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Priest"))
        {
            gameManager.TrySwitchRoom(exitedRoom , exitedRoomCage);
        }
    }
}
