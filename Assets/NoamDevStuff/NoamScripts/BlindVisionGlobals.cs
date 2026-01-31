using UnityEngine;

public class BlindVisionGlobals : MonoBehaviour
{
    [Header("Assign these")]
    public Transform blindPlayerRoot;        // your blind character root (or camera)
    public SenseProbe senseProbe;            // the probe you already use
    public Material blindBuildingMaterial;   // MUST be the same material used in Render Objects override (M_BlindBuilding)

    static readonly int PlayerPosID = Shader.PropertyToID("_Blind_PlayerPos");
    static readonly int RadiusID    = Shader.PropertyToID("_Blind_Radius");

    void LateUpdate()
    {
        if (!blindBuildingMaterial) return;

        Vector3 p = blindPlayerRoot ? blindPlayerRoot.position : transform.position;
        float r = senseProbe ? senseProbe.defaultRadius : 4f;

        blindBuildingMaterial.SetVector(PlayerPosID, new Vector4(p.x, p.y, p.z, 1f));
        blindBuildingMaterial.SetFloat(RadiusID, r);
    }
}