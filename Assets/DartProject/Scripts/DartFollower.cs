using UnityEngine;

public class DartFollower : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetBody;

    public Vector3 offset = new Vector3(0f, 1.5f, -4f);
    public float moveSpeed = 5f;
    public float turnSpeed = 7f;
    public float lookAhead = 2f;
    public bool useVelocityLook = true;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 wantedPos = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, wantedPos, moveSpeed * Time.deltaTime);

        Vector3 forward = target.forward;
        if (useVelocityLook && targetBody != null && targetBody.linearVelocity.sqrMagnitude > 0.1f)
        {
            forward = targetBody.linearVelocity.normalized;
        }

        Vector3 lookPoint = target.position + forward * lookAhead;
        Quaternion wantedRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRot, turnSpeed * Time.deltaTime);
    }
}
