using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float mouseSensitivity = 3f;
    public Transform cameraPivot;

    public static float CurrentYaw { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        CurrentYaw += mouseX;

        cameraPivot.localRotation = Quaternion.Euler(0f, CurrentYaw, 0f);
    }
}
