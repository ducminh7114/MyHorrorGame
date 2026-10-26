
using UnityEngine;
using System.Collections;

public class PickUpItem : MonoBehaviour
{
    [Header("Flashlight")]
    public GameObject inttext;
    public GameObject flashlight_table;
    public GameObject flashlight_player;
    public AudioSource pickup;
    public AudioClip pickupSound;
    public bool interactable;

    [Header("Save IDs")]
    public string flashlightPickupID = "Flashlight_PickedUp";

    private bool pickedUp;

    IEnumerator Start()
    {
        if (inttext != null)
            inttext.SetActive(false);

        // Wait for the save manager to restore the saved state.
        yield return null;
        yield return null;

        if (GameSaveManager.Instance != null &&
            GameSaveManager.Instance.TryGetObjectState(
                flashlightPickupID, out bool wasPickedUp))
        {
            pickedUp = wasPickedUp;

            if (pickedUp)
            {
                if (flashlight_table != null)
                    flashlight_table.SetActive(false);

                if (flashlight_player != null)
                    flashlight_player.SetActive(true);

                if (inttext != null)
                    inttext.SetActive(false);

                interactable = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("MainCamera"))
        {
            if (inttext != null)
                inttext.SetActive(true);

            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (inttext != null)
                inttext.SetActive(false);

            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && !pickedUp &&
            Input.GetKeyDown(KeyCode.E))
        {
            pickedUp = true;
            interactable = false;

            if (GameSaveManager.Instance != null)
            {
                GameSaveManager.Instance.SetObjectState(
                    flashlightPickupID, true);
            }

            if (flashlight_table != null)
                SaveObjectState.SetActive(flashlight_table, false);

            if (flashlight_player != null)
                flashlight_player.SetActive(true);

            if (inttext != null)
                inttext.SetActive(false);

            if (pickup != null)
                { pickup.PlayOneShot(pickupSound); }
        }
    }
}

