using UnityEngine;

public class WhellSC : MonoBehaviour
{
    public WheelCollider whellCollider;
    public Transform whellMesh;

    void Update()
    {
        if (whellCollider == null || whellMesh == null) return;

        Vector3 pos;
        Quaternion rot;
        whellCollider.GetWorldPose(out pos, out rot);
        whellMesh.position = pos;
        whellMesh.rotation = rot;
    }
}
