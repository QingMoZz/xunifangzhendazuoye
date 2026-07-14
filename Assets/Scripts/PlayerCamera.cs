using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -5f);
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);
    public float followSpeed = 5f;

    [Header("视角设置")]
    public float mouseSensitivity = 3f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("缩放设置")]
    public float zoomSpeed = 5f;
    public float minDistance = 1.5f;
    public float maxDistance = 10f;

    [Header("鼠标设置")]
    public bool lockCursor = true;

    private float yaw;
    private float pitch;
    private float distance;

    void Start()
    {
        if (target == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                target = player.transform;
            }
        }

        distance = Mathf.Max(offset.magnitude, 0.1f);
        Vector3 initialAngles = Quaternion.LookRotation(-offset.normalized).eulerAngles;
        yaw = initialAngles.y;
        pitch = initialAngles.x > 180f
            ? initialAngles.x - 360f
            : initialAngles.x;

        SetCursorLocked(lockCursor);
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLocked(false);
        }
        else if (lockCursor && Input.GetMouseButtonDown(0))
        {
            SetCursorLocked(true);
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            yaw = Mathf.Repeat(yaw, 360f);

            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = target.position + lookAtOffset;
        Vector3 targetPosition = focusPoint + rotation * Vector3.back * distance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
        transform.LookAt(focusPoint);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lockCursor)
        {
            SetCursorLocked(true);
        }
    }

    private void SetCursorLocked(bool isLocked)
    {
        Cursor.lockState = isLocked
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}
