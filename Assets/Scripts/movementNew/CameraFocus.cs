using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour
{
    public playerMovement player;
    public Camera playerCamera;

    public void FocusOnTarget(
    Transform target,
    float focusTime,
    float zoomFOV = 40f,
    float zoomOutTime = 1f)
    
    {
        StartCoroutine(
            FocusRoutine(
                target,
                focusTime,
                zoomFOV));
    }

    IEnumerator FocusRoutine(
        Transform target,
        float focusTime,
        float zoomFOV)
    {
        player.canLook = false;

        float originalFOV = playerCamera.fieldOfView;

        Quaternion startRot =
            playerCamera.transform.rotation;

        Quaternion targetRot =
            Quaternion.LookRotation(
                target.position -
                playerCamera.transform.position);

        float t = 0f;
        float duration = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;

            playerCamera.transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    targetRot,
                    t / duration);

            playerCamera.fieldOfView =
                Mathf.Lerp(
                    originalFOV,
                    zoomFOV,
                    t / duration);

            yield return null;
        }

        yield return new WaitForSeconds(focusTime);

        // Smoothly return FOV
        float zoomOutTime = 1f;
        float zoomT = 0f;

        while (zoomT < zoomOutTime)
        {
            zoomT += Time.deltaTime;

            playerCamera.fieldOfView =
                Mathf.Lerp(
                    zoomFOV,
                    originalFOV,
                    zoomT / zoomOutTime);

            yield return null;
        }

        // Prevent snapping when player regains control
        player.SyncCameraRotation();

        player.canLook = true;
    }
}