using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
public class Player : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Transform cameraTransform;

    [Header("动画设置")]
    public string idleAnimation = "wait";
    public string runAnimation = "run";

    private CharacterController controller;
    private Animation characterAnimation;
    private bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        characterAnimation = GetComponent<Animation>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        SetAnimationLoop(idleAnimation);
        SetAnimationLoop(runAnimation);
        PlayAnimation(idleAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (cameraTransform != null)
        {
            forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();
        }

        Vector3 move = forward * vertical + right * horizontal;
        move = Vector3.ClampMagnitude(move, 1f);

        bool hasMovement = move.sqrMagnitude > 0.001f;
        if (hasMovement)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (hasMovement != isWalking)
        {
            isWalking = hasMovement;
            PlayAnimation(isWalking ? runAnimation : idleAnimation);
        }

        controller.SimpleMove(move * moveSpeed);
    }

    private void PlayAnimation(string clipName)
    {
        if (characterAnimation != null && characterAnimation[clipName] != null)
        {
            characterAnimation.CrossFade(clipName);
        }
    }

    private void SetAnimationLoop(string clipName)
    {
        if (characterAnimation != null && characterAnimation[clipName] != null)
        {
            characterAnimation[clipName].wrapMode = WrapMode.Loop;
        }
    }
}