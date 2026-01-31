using UnityEngine;

public class Objective : MonoBehaviour
{
    public void OnResetDay(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }
}
