using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera priestCamera;
    [SerializeField] private Camera angleCamera;

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
    
    
}
