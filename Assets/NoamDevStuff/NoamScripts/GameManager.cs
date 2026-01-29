using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DebugUI debugUI;
    [SerializeField] private Camera priestCamera;
    [SerializeField] private Camera angleCamera;

    [Header("Priest")] 
    [SerializeField] private string priestCurrentRoom = "";
    
    // fields
    private int _cameraIndex;
    private Camera[] _cameras;
    private void Awake()
    {
        _cameras = new[] { priestCamera, angleCamera };
    }
    
    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        _cameras[_cameraIndex].gameObject.SetActive(false);
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
        _cameras[_cameraIndex].gameObject.SetActive(true);
    }
    public void TrySwitchRoom(string newRoom)
    {
        priestCurrentRoom = newRoom;
        
        // debug ui:
        debugUI.OnChangeText(TextContext.CurrentRoomText , priestCurrentRoom);
    }
}

