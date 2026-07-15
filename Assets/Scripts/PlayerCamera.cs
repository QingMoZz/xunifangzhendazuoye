using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -5f);
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);
    public float followSpeed = 10f;

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
    private bool isCursorFree;

    void Start()
    {
        if (target == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    target = playerObject.transform;
                }
            }
        }

        distance = Mathf.Clamp(offset.magnitude, minDistance, maxDistance);
        InitializeOrbitAngles();

        if (target != null)
        {
            SnapToTarget();
        }

        if (lockCursor)
        {
            StartCoroutine(LockCursorDelayed());
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleCursorLock();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            yaw = Mathf.Repeat(yaw, 360f);

            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        Vector3 focusPoint = target.position + lookAtOffset;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = focusPoint + orbitRotation * Vector3.back * distance;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
        transform.rotation = Quaternion.LookRotation(focusPoint - transform.position);
    }

    private void InitializeOrbitAngles()
    {
        Vector3 orbitOffset = offset;
        if (orbitOffset.sqrMagnitude < 0.001f)
        {
            orbitOffset = new Vector3(0f, 3f, -5f);
        }

        float horizontalDistance = new Vector2(orbitOffset.x, orbitOffset.z).magnitude;
        yaw = Mathf.Atan2(orbitOffset.x, orbitOffset.z) * Mathf.Rad2Deg;
        pitch = Mathf.Atan2(orbitOffset.y, horizontalDistance) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void SnapToTarget()
    {
        Vector3 focusPoint = target.position + lookAtOffset;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = focusPoint + orbitRotation * Vector3.back * distance;
        transform.rotation = Quaternion.LookRotation(focusPoint - transform.position);
    }

    private void HandleCursorLock()
    {
        if (!lockCursor)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorFree = true;
            SetCursorLocked(false);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isCursorFree = false;
        }

        if (!isCursorFree)
        {
            SetCursorLocked(true);
        }
    }

    private IEnumerator LockCursorDelayed()
    {
        yield return null;
        if (lockCursor && !isCursorFree)
        {
            SetCursorLocked(true);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && lockCursor && !isCursorFree)
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
