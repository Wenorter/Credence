using System;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string enteredRoom;
    [SerializeField] private string exitedRoom;
    [SerializeField] private GameManager gameManager;
    
    private void OnTriggerEnter(Collider other)
    {
        gameManager.TrySwitchRoom(enteredRoom);
    }
    private void OnTriggerExit(Collider other)
    {
        gameManager.TrySwitchRoom(exitedRoom);
    }
}
