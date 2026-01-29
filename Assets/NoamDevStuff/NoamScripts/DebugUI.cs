using System;
using UnityEngine;
public class DebugUI : MonoBehaviour
{
    public void OnChangeText(TextContext textContext, string text)
    {
        switch (textContext)
        {
            case TextContext.CurrentRoomText:
                Debug.Log(text);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(textContext), textContext, null);
        }    
    }
}

public enum TextContext
{
    CurrentRoomText,
    
}