using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [Tooltip("WheelCollider components for front/back, left/right wheels")]
    public WheelCollider rightFrontWheel, leftFrontWheel, rightBackWheel, leftBackWheel;

    [Header("Vehicle Options")]
    [Tooltip("Different vehicle setups (speed, torque, drivetrain etc.)")]
    public VehicleStats[] vehicleOptions;

    [Tooltip("Index of the currently active vehicle in the array")]
    public int selectedIndex = 0;

    Rigidbody rb;

    float horizontalInput; 
    float verticalInput;   

    VehicleStats activeStats;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // aracin stabilitesi icin agırlık merkezini asagıya cekiyoruz
        rb.centerOfMass = new Vector3(0f, -0.3f, 0f);
    }

    void Update()
    {
        ReadPlayerInput();
        HandleVehicleSwitchInput();
    }

    void ReadPlayerInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    // Arac ozellikleri arasinda gecis yap
    void HandleVehicleSwitchInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedIndex = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedIndex = 1;
    }

    void FixedUpdate()
    {
        activeStats = vehicleOptions[selectedIndex];
        if (activeStats == null) return; 

        // metre / saniye hizimiz
        float currentSpeed = rb.linearVelocity.magnitude;
        float maxSpeed = activeStats.MaxSpeedMS;

        // Higher acceleration at low speed, lower near max speed
        float accelFactor = activeStats.EvaluateAccelFactor(currentSpeed);

        // Desired motor torque based on input and curve
        float desiredMotorTorque = activeStats.baseMotorTorque * verticalInput * accelFactor;

        // When going forward and already at max speed, cut extra torque
        if (verticalInput > 0f && currentSpeed >= maxSpeed)
        {
            desiredMotorTorque = 0f;
        }

        // Simple braking (Space key)
        float brakeTorque;
        if (Input.GetKey(KeyCode.Space))
            brakeTorque = activeStats.brakeTorque;
        else
            brakeTorque = 0;

        ApplySteering();
        ApplyBrakes(brakeTorque);
        ApplyDriveTorque(desiredMotorTorque);
        ApplyDownforce(currentSpeed);
    }
    // Apply steering angle only to front wheels
    void ApplySteering()
    {
        float steerAngle = activeStats.steerSpeed * horizontalInput;
        rightFrontWheel.steerAngle = steerAngle;
        leftFrontWheel.steerAngle = steerAngle;
    }

    // Apply brake torque to all wheels
    void ApplyBrakes(float brakeTorque)
    {
        rightFrontWheel.brakeTorque = brakeTorque;
        leftFrontWheel.brakeTorque = brakeTorque;
        rightBackWheel.brakeTorque = brakeTorque;
        leftBackWheel.brakeTorque = brakeTorque;
    }

    // Distribute motor torque to wheels based on drive type
    void ApplyDriveTorque(float desiredMotorTorque)
    {
        // Önce tüm motor torklarını sıfırla
        rightFrontWheel.motorTorque = 0f;
        leftFrontWheel.motorTorque = 0f;
        rightBackWheel.motorTorque = 0f;
        leftBackWheel.motorTorque = 0f;

        switch (activeStats.driveType)
        {
            case VehicleStats.DriveType.FWD: // onden cekis
                rightFrontWheel.motorTorque = desiredMotorTorque * 0.5f;
                leftFrontWheel.motorTorque = desiredMotorTorque * 0.5f;
                break;

            case VehicleStats.DriveType.RWD: // Arkadan itis
                rightBackWheel.motorTorque = desiredMotorTorque * 0.5f;
                leftBackWheel.motorTorque = desiredMotorTorque * 0.5f;
                break;

            case VehicleStats.DriveType.AWD: // 4x4
                rightFrontWheel.motorTorque = desiredMotorTorque * 0.25f;
                leftFrontWheel.motorTorque = desiredMotorTorque * 0.25f;
                rightBackWheel.motorTorque = desiredMotorTorque * 0.25f;
                leftBackWheel.motorTorque = desiredMotorTorque * 0.25f;
                break;
        }
    }

    void ApplyDownforce(float currentSpeed)
    {
        rb.AddForce(-transform.forward * activeStats.downforce * currentSpeed);
    }
}
