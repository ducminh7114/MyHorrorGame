using UnityEngine;

public class Ladder : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        playerMovement player = other.GetComponent<playerMovement>();

        if (player != null)
        {
            Debug.Log("Player entered ladder");
            player.isClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerMovement player = other.GetComponent<playerMovement>();

        if (player != null)
        {
            Debug.Log("Player exited ladder");
            player.isClimbing = false;
        }
    }
}