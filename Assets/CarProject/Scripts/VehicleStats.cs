using UnityEngine;

[CreateAssetMenu(fileName = "VehicleStats", menuName = "Racing/Vehicle Stats", order = 0)]
public class VehicleStats : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "Vehicle";

    [Header("Performance")]
    [Tooltip("Maximum speed in km/h")] public float maxSpeedKmh = 160f;
    [Tooltip("Base motor torque applied to drive wheels")]
    public float baseMotorTorque = 1200f;
    [Tooltip("Steer speed (deg)")] public float steerSpeed = 25f;
    [Tooltip("Brake torque when braking")] public float brakeTorque = 2000f;
    [Tooltip("Downforce coefficient, increases with speed")] public float downforce = 30f;

    [Header("Acceleration Curve")]
    [Tooltip("y = acceleration factor, x = normalized speed (0..1)")]
    public AnimationCurve accelerationVsNormalizedSpeed = new AnimationCurve(
        new Keyframe(0f, 1.0f, 0f, -1.5f),
        new Keyframe(0.3f, 0.75f),
        new Keyframe(0.7f, 0.35f),
        new Keyframe(1f, 0.1f)
    );

    public enum DriveType { FWD, RWD, AWD }
    [Header("Drivetrain")]
    public DriveType driveType = DriveType.AWD;

    public float MaxSpeedMS => maxSpeedKmh / 3.6f;
    public float EvaluateAccelFactor(float currentSpeedMS)
    {
        if (MaxSpeedMS <= 0.01f) return 1f;
        float t = Mathf.Clamp01(currentSpeedMS / MaxSpeedMS);
        return Mathf.Clamp01(accelerationVsNormalizedSpeed.Evaluate(t));
    }
}
