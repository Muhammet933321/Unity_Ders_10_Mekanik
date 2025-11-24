using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider rightFrontWhell, leftFrontWhell, rightBackWhell, leftBackWhell;

    [Header("Vehicle Variants")]
    public VehicleStats[] vehicleOptions;
    [Tooltip("Index of the active vehicle in vehicleOptions")] public int selectedIndex = 0;

    Rigidbody rb;
    float horizontalInput, verticalInput;

    VehicleStats ActiveStats
    {
        get
        {
            if (vehicleOptions != null && vehicleOptions.Length > 0)
            {
                int i = Mathf.Clamp(selectedIndex, 0, vehicleOptions.Length - 1);
                return vehicleOptions[i];
            }
            return null;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.3f, 0); // a little stability
    }

    void Update()
    {
        // Basic input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Quick switch between two vehicles with keys 1 and 2 (if present)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedIndex = 1;
    }

    void FixedUpdate()
    {
        var stats = ActiveStats;
        if (stats == null) return;

        float speed = rb.linearVelocity.magnitude; // m/s
        float maxSpeed = Mathf.Max(1f, stats.MaxSpeedMS);

        // Evaluate acceleration factor (strong at low speed, weaker near max)
        float accelFactor = stats.EvaluateAccelFactor(speed);

        // Compute desired motor torque based on input and curve
        float desiredTorque = stats.baseMotorTorque * verticalInput * accelFactor;

        // Respect forward max speed
        if (verticalInput > 0f && speed >= maxSpeed)
        {
            desiredTorque = 0f;
        }

        // Simple braking (Space)
        bool braking = Input.GetKey(KeyCode.Space);
        float brake = braking ? stats.brakeTorque : 0f;

        // Apply steering (front wheels)
        float steer = stats.steerSpeed * horizontalInput;
        rightFrontWhell.steerAngle = steer;
        leftFrontWhell.steerAngle = steer;

        // Reset torques
        rightFrontWhell.brakeTorque = brake;
        leftFrontWhell.brakeTorque = brake;
        rightBackWhell.brakeTorque = brake;
        leftBackWhell.brakeTorque = brake;

        rightFrontWhell.motorTorque = 0f;
        leftFrontWhell.motorTorque = 0f;
        rightBackWhell.motorTorque = 0f;
        leftBackWhell.motorTorque = 0f;

        // Drivetrain distribution
        switch (stats.driveType)
        {
            case VehicleStats.DriveType.FWD:
                rightFrontWhell.motorTorque = desiredTorque * 0.5f;
                leftFrontWhell.motorTorque = desiredTorque * 0.5f;
                break;
            case VehicleStats.DriveType.RWD:
                rightBackWhell.motorTorque = desiredTorque * 0.5f;
                leftBackWhell.motorTorque = desiredTorque * 0.5f;
                break;
            case VehicleStats.DriveType.AWD:
                rightFrontWhell.motorTorque = desiredTorque * 0.25f;
                leftFrontWhell.motorTorque = desiredTorque * 0.25f;
                rightBackWhell.motorTorque = desiredTorque * 0.25f;
                leftBackWhell.motorTorque = desiredTorque * 0.25f;
                break;
        }

        // Add simple downforce proportional to speed for stability on hills
        rb.AddForce(-transform.up * stats.downforce * speed, ForceMode.Force);
    }
}
