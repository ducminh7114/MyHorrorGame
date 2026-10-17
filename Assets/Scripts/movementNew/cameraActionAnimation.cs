using UnityEngine;

public class cameraActionAnimation : MonoBehaviour
{
    [Header("References")]
    public Animator cameraAnimator;

    [Header("Animation Settings")]
    public float movementThreshold = 0.01f;

    private string currentAnimation = "";

    void Update()
    {
        if (cameraAnimator == null)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool isMoving =
            Mathf.Abs(horizontal) > movementThreshold ||
            Mathf.Abs(vertical) > movementThreshold;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        string nextAnimation;

        if (!isMoving)
        {
            nextAnimation = "idle";
        }
        else if (isRunning)
        {
            nextAnimation = "run";
        }
        else
        {
            nextAnimation = "walk";
        }

        PlayAnimation(nextAnimation);
    }

    void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        cameraAnimator.ResetTrigger("idle");
        cameraAnimator.ResetTrigger("walk");
        cameraAnimator.ResetTrigger("run");

        cameraAnimator.SetTrigger(animationName);

        currentAnimation = animationName;
    }
}
