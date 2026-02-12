using UnityEngine;

public class AngelLogic : MonoBehaviour
{
    public void OnPriestChangedRooms(Transform newPos)
    {
        transform.position = newPos.position;
    }
}
