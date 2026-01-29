using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DebugUI debugUI;
    [SerializeField] private Camera priestCamera;
    [SerializeField] private Camera angelCamera;
    [SerializeField] private GameObject priestInput;
    [SerializeField] private GameObject angelInput;
    [SerializeField] private GameObject angel;
    [SerializeField] private GameObject priest;
    [SerializeField] private BoxCollider cage1;
    [Header("Priest")] 
    [SerializeField] private string priestCurrentRoom = "";
    
    // fields
    private int _cameraIndex;
    private Camera[] _cameras;
    private GameObject[] _inputObj;
    private void Awake()
    {
        _cameras = new[] { priestCamera, angelCamera };
        _inputObj = new[] { priestInput, angelInput };
    }

    private void Start()
    {
        TrySwitchRoom("room1", cage1);
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            OnSwitchCamera();
        }
    }
    
    public void OnSwitchCamera()
    {
        _cameras[_cameraIndex].gameObject.SetActive(false);
        _inputObj[_cameraIndex].SetActive(false);
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
        _cameras[_cameraIndex].gameObject.SetActive(true);
        _inputObj[_cameraIndex].SetActive(true);
    }
    public void TrySwitchRoom(string newRoom , BoxCollider newRoomCage)
    {
        priestCurrentRoom = newRoom;
        angel.GetComponent<StayInsideBox>().cage = newRoomCage;
        angel.GetComponent<CharacterController>().enabled = false;  
        angel.transform.position = newRoomCage.bounds.center;
        angel.GetComponent<CharacterController>().enabled = true;  
        // debug ui:
        debugUI.OnChangeText(TextContext.CurrentRoomText , priestCurrentRoom);
    }
}

