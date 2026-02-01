using System;
using UnityEngine;
public class DebugUI : MonoBehaviour
{
    public static void OnChangeText(TextContext textContext, string text)
    {
        switch (textContext)
        {
            case TextContext.CurrentRoomText:
                Debug.Log($"Current Room: {text}");
                break;
            case TextContext.PriestHp:
                Debug.Log($"Priest hp: {text}");
                break;
            case TextContext.AngelSpooked:
                Debug.Log($"Current Angel Fear: {text}");
                break;
            case TextContext.AngelStunned:
                Debug.Log($"Angel Reached: {text} fear and got stunned!");
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(textContext), textContext, null);
        }    
    }
}

public enum TextContext
{
    CurrentRoomText,
    PriestHp,
    AngelSpooked,
    AngelStunned,
}