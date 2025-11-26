using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DartController : MonoBehaviour
{
    public float rotationSpeed = 60f;
    public float pitchLimit = 60f;
    public float yawLimit = 90f;

    public float chargeSpeed = 1f;
    public float minForce = 5f;
    public float maxForce = 25f;

    public string stickTag = "DartBoard";

    private float pitch;
    private float yaw;
    private float charge;
    private bool chargingUp = true;
    private bool thrown;
    private bool stuck;

    private Rigidbody body;
    private Vector3 startPos;
    private Quaternion startRot;
    private float startPitch;
    private float startYaw;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;

        startPos = transform.position;
        startRot = transform.rotation;
        startPitch = NormalizeAngle(transform.eulerAngles.x);
        startYaw = NormalizeAngle(transform.eulerAngles.y);
        pitch = startPitch;
        yaw = startYaw;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDart();
        }

        if (stuck)
            return;

        if (thrown)
        {
            AlignWithVelocity();
            return;
        }

        ReadRotation();
        ReadCharge();
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void ReadRotation()
    {
        float pitchInput = 0f;
        if (Input.GetKey(KeyCode.W)) pitchInput -= 1f;
        if (Input.GetKey(KeyCode.S)) pitchInput += 1f;
        pitch += pitchInput * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.D)) yawInput += 1f;
        if (Input.GetKey(KeyCode.A)) yawInput -= 1f;
        yaw += yawInput * rotationSpeed * Time.deltaTime;
        yaw = Mathf.Clamp(yaw, -yawLimit, yawLimit);
    }

    private void ReadCharge()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float direction = chargingUp ? 1f : -1f;
            charge += direction * chargeSpeed * Time.deltaTime;
            if (charge >= 1f)
            {
                charge = 1f;
                chargingUp = false;
            }
            else if (charge <= 0f)
            {
                charge = 0f;
                chargingUp = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            thrown = true;
            Launch();
        }
    }

    private void Launch()
    {
        body.isKinematic = false;
        body.useGravity = true;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        float force = Mathf.Lerp(minForce, maxForce, Mathf.Clamp01(charge));
        body.AddForce(transform.forward * force);
    }

    private void AlignWithVelocity()
    {
        if (body.linearVelocity.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(body.linearVelocity.normalized, Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!thrown || stuck)
            return;

        if (!collision.collider.CompareTag(stickTag))
            return;

        StickToSurface(collision);
    }

    private void StickToSurface(Collision collision)
    {
        stuck = true;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
        body.isKinematic = true;
    }

    private void ResetDart()
    {
        stuck = false;
        thrown = false;
        chargingUp = true;
        charge = 0f;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
        body.isKinematic = false;

        transform.SetPositionAndRotation(startPos, startRot);
        pitch = startPitch;
        yaw = startYaw;
        transform.rotation = startRot;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;
        if (angle < -180f)
            angle += 360f;
        return angle;
    }
}
