using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamBlob : MonoBehaviour
{
    public Animator cameraAnim;

    void Update()
    {
        bool moving =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        bool running = Input.GetKey(KeyCode.LeftShift);

        if (moving)
        {
            if (running)
            {
                cameraAnim.ResetTrigger("idle");
                cameraAnim.ResetTrigger("walk");
                cameraAnim.SetTrigger("run");
            }
            else
            {
                cameraAnim.ResetTrigger("idle");
                cameraAnim.ResetTrigger("run");
                cameraAnim.SetTrigger("walk");
            }
        }
        else
        {
            cameraAnim.ResetTrigger("walk");
            cameraAnim.ResetTrigger("run");
            cameraAnim.SetTrigger("idle");
        }
    }
}
