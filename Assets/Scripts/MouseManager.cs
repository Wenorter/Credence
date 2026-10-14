using System;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private bool hideCursorOnStart = true;
    
    private void Awake()
    {
        Cursor.lockState = lockCursorOnStart ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hideCursorOnStart;
    }
}
