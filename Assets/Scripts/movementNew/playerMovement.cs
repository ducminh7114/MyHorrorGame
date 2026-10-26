using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class playerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 4.5f;
    public float runningSpeed = 7.5f;
    public float crouchSpeed = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Camera")]
    public Camera playerCamera;
    public Transform lookPivot;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 80.0f;
    
    [Header("Crouch Settings")]
    public float normalHeight = 1.8f;
    public float crouchHeight = 0.9f;

    [HideInInspector]
    public bool canLook = true;

    private Vector3 normalCenter;
    private Vector3 crouchCenter;

    [SerializeField] private float normalCamY = 0f;
    [SerializeField] private float crouchCamY = -0.8f;

    public Transform cameraParent;

    [Header("Ladder")]
    public bool isClimbing = false;
    public float climbSpeed = 3f;

    [Header("Animation")]
    public Animator animator;

    [Header("Footstep Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip walkFootstepSound;

    [Header("Footstep Timing")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    [Header("Footstep Speed")]
    public float walkSoundSpeed = 1f;
    public float runSoundSpeed = 2f;

    [Range(0f, 1f)]
    public float footstepVolume = 0.8f;

    private float footstepTimer = 0f;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    [HideInInspector]
    public float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private bool isCrouched = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Always keep correct center
        normalCenter = new Vector3(0, normalHeight / 2f, 0);
        crouchCenter = new Vector3(0, crouchHeight / 2f, 0);

        characterController.height = normalHeight;
        characterController.center = normalCenter;

        isCrouched = false;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCrouch();
        HandleAnimation();
        HandleFootstepAudio();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = isCrouched
            ? crouchSpeed
            : (isRunning ? runningSpeed : walkingSpeed);

        float curSpeedX = canMove ? speed * Input.GetAxisRaw("Vertical") : 0;
        float curSpeedY = canMove ? speed * Input.GetAxisRaw("Horizontal") : 0;

        Debug.Log(Input.GetAxis("Vertical"));

        float movementY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouched && !isClimbing)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementY;
        }

        if (isClimbing)
        {
            moveDirection.y = Input.GetAxisRaw("Vertical") * climbSpeed;
        }
        else
        {
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (!canLook) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation =
            Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(
            0, Input.GetAxis("Mouse X") * lookSpeed, 0
        );
    }

    public void SyncCameraRotation()
    {
        if (lookPivot == null) return;

        float pitch = lookPivot.localEulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        rotationX = pitch;
    }


    void HandleCrouch()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl))
            return;

        if (isCrouched)
        {
            // Calculate the capsule needed to stand up.
            float radius = characterController.radius * 0.95f;

            Vector3 bottom = transform.position +
                             Vector3.up * radius;

            Vector3 top = transform.position +
                          Vector3.up * (normalHeight - radius);

            Collider[] hits = Physics.OverlapCapsule(
                bottom,
                top,
                radius,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore
            );

            bool blocked = false;

            foreach (Collider hit in hits)
            {
                // Ignore the player's own collider.
                if (hit == characterController ||
                    hit.transform.IsChildOf(transform))
                    continue;

                blocked = true;
                Debug.Log("Cannot stand up: " + hit.name);
                break;
            }

            if (blocked)
                return;

            // Stand up. Center and height preserve the feet position.
            characterController.height = normalHeight;
            characterController.center = normalCenter;

            isCrouched = false;

            if (cameraParent != null)
                cameraParent.localPosition =
                    new Vector3(0f, normalCamY, 0f);
        }
        else
        {
            // Crouch down.
            characterController.height = crouchHeight;
            characterController.center = crouchCenter;

            isCrouched = true;

            if (cameraParent != null)
                cameraParent.localPosition =
                    new Vector3(0f, crouchCamY, 0f);
        }
    }

    void SetCrouch(bool crouch)
    {
        isCrouched = crouch;

        characterController.height =
            crouch ? crouchHeight : normalHeight;

        characterController.center =
            crouch ? crouchCenter : normalCenter;

        if (cameraParent != null)
        {
            cameraParent.localPosition = new Vector3(
                0f,
                crouch ? crouchCamY : normalCamY,
                0f
            );
        }
    }


    public float GetCameraPitch()
    {
        return rotationX;
    }

    public void SetCameraPitch(float pitch)
    {
        rotationX = Mathf.Clamp(pitch, -lookXLimit, lookXLimit);

        if (lookPivot != null)
        {
            lookPivot.localRotation =
                Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    void HandleAnimation()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        bool isMoving = Mathf.Abs(vertical) > 0.1f ||
                        Mathf.Abs(horizontal) > 0.1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) &&
                         isMoving &&
                         !isCrouched &&
                         !isClimbing;

        if (animator == null) return;

        if (!isMoving || !canMove)
        {
            animator.ResetTrigger("walk");
            animator.ResetTrigger("run");
            animator.SetTrigger("idle");
        }
        else if (isRunning)
        {
            animator.ResetTrigger("idle");
            animator.ResetTrigger("walk");
            animator.SetTrigger("run");
        }
        else
        {
            animator.ResetTrigger("idle");
            animator.ResetTrigger("run");
            animator.SetTrigger("walk");
        }
    }

    void HandleFootstepAudio()
    {
        bool isMoving =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);

        // Stop the current sound immediately when movement stops.
        if (!isMoving || !canMove || isClimbing)
        {
            footstepTimer = 0f;

            if (footstepAudioSource != null &&
                footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }

            return;
        }

        if (footstepAudioSource == null ||
            walkFootstepSound == null)
        {
            return;
        }

        float stepInterval = isRunning
            ? runStepInterval
            : walkStepInterval;

        float soundSpeed = isRunning
            ? runSoundSpeed
            : walkSoundSpeed;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            // Stop the previous footstep before starting another.
            footstepAudioSource.Stop();

            footstepAudioSource.clip = walkFootstepSound;
            footstepAudioSource.pitch = soundSpeed;
            footstepAudioSource.volume = footstepVolume;
            footstepAudioSource.loop = false;

            footstepAudioSource.Play();

            footstepTimer = stepInterval;
        }
    }
}