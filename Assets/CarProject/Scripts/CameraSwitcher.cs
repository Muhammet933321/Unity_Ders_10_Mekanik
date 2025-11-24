using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Tooltip("Assign at least 3 cameras. Only the active one will be enabled.")]
    public Camera[] cameras;
    public int activeIndex = 0;
    public KeyCode switchKey = KeyCode.C;

    void Start()
    {
        ApplyActive();
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            NextCamera();
        }
    }

    public void NextCamera()
    {
        if (cameras == null || cameras.Length == 0) return;
        activeIndex = (activeIndex + 1) % cameras.Length;
        ApplyActive();
    }

    void ApplyActive()
    {
        if (cameras == null) return;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null) cameras[i].enabled = (i == activeIndex);
        }
    }
}
