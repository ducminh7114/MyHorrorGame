using UnityEngine;
using System.Collections;

public class BeginCutscene : MonoBehaviour
{
    public GameObject cutsceneCam;

    [Header("Player")]
    public GameObject player;               // Drag the Player GameObject here
    public Transform endPosition;
    public playerMovement playerMovement;   // Drag the playerMovement component here

    public float cutsceneTime = 5f;

    public void StartCutscene()
    {
        StartCoroutine(Cutscene());
    }

    IEnumerator Cutscene()
    {
        // Disable player controls
        playerMovement.enabled = false;

        // Show cutscene camera
        cutsceneCam.SetActive(true);

        // OPTIONAL: Hide the player during the cutscene
        player.SetActive(false);

        yield return new WaitForSeconds(cutsceneTime);

        // Move player while inactive
        player.transform.SetPositionAndRotation(
            endPosition.position,
            endPosition.rotation
        );

        // Show player again
        player.SetActive(true);

        // Reset CharacterController
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            yield return null;
            cc.enabled = true;
        }

        // Hide cutscene camera
        cutsceneCam.SetActive(false);

        // Enable controls
        playerMovement.enabled = true;
        playerMovement.SyncCameraRotation();
    }
}