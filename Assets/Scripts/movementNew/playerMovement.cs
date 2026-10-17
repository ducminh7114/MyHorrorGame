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
    public float lookSpeed = 2.0f;
    public float lookXLimit = 80.0f;

    [Header("Crouch Settings")]
    public float normalHeight = 1.8f;
    public float crouchHeight = 0.9f;

    [HideInInspector]
    public bool canLook = true;

    private Vector3 normalCenter;
    private Vector3 crouchCenter;

    private float normalCamY = 1.6f;
    private float crouchCamY = 0.8f;

    public Transform cameraParent;

    [Header("Ladder")]
    public bool isClimbing = false;
    public float climbSpeed = 3f;


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
        Vector3 angles = playerCamera.transform.localEulerAngles;

        float pitch = angles.x;

        if (pitch > 180)
            pitch -= 360;

        rotationX = pitch;
    }

    void HandleCrouch()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl)) return;

        if (isCrouched)
        {
            // Try to stand (ceiling check)
            float checkDistance = normalHeight - characterController.height;

            Vector3 origin = transform.position + Vector3.up * characterController.height;

            if (!Physics.SphereCast(origin, characterController.radius * 0.9f, Vector3.up, out RaycastHit hit, checkDistance))
            {
                // Stand up
                float oldHeight = characterController.height;

                characterController.height = normalHeight;
                characterController.center = normalCenter;

                // Keep feet grounded
                float diff = normalHeight - oldHeight;
                characterController.Move(Vector3.up * (diff / 2f));

                isCrouched = false;

                // Camera
                cameraParent.localPosition = new Vector3(0, normalCamY, 0);
            }
            else
            {
                Debug.Log("Blocked above, can't stand");
                Debug.Log(hit.collider.name);
                Debug.Log("Blocked by: " + hit.collider.name);
            }
        }
        else
        {
            // Crouch
            float oldHeight = characterController.height;

            characterController.height = crouchHeight;
            characterController.center = crouchCenter;

            // Keep feet grounded
            float diff = oldHeight - crouchHeight;
            characterController.Move(Vector3.down * (diff / 2f));

            isCrouched = true;

            // Camera
            cameraParent.localPosition = new Vector3(0, crouchCamY, 0);
            Debug.Log(playerCamera.transform.localPosition);
        }
    }

    public float GetCameraPitch()
    {
        return rotationX;
    }

    public void SetCameraPitch(float pitch)
    {
        rotationX = pitch;

        playerCamera.transform.localRotation =
            Quaternion.Euler(rotationX, 0f, 0f);
    }
}