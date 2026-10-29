
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

    [Header("Save ID - must be unique")]
    public string flashlightPickupID = "Flashlight_PickedUp";

    private bool interactable;
    private bool pickedUp;
    private bool restoring;

    IEnumerator Start()
    {
        restoring = true;

        if (inttext != null)
            inttext.SetActive(false);

        // Wait for save loading and scene initialization.
        yield return null;
        yield return null;

        if (GameSaveManager.Instance != null &&
            GameSaveManager.Instance.TryGetObjectState(
                flashlightPickupID, out bool wasPickedUp))
        {
            pickedUp = wasPickedUp;
        }

        ApplyPickupState();
        restoring = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (restoring || pickedUp)
            return;

        if (!other.CompareTag("MainCamera"))
            return;

        interactable = true;

        if (inttext != null)
            inttext.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
            return;

        interactable = false;

        if (inttext != null)
            inttext.SetActive(false);
    }

    void Update()
    {
        if (restoring || pickedUp || !interactable)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        pickedUp = true;
        interactable = false;

        // Save only this pickup's collected flag.
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SetObjectState(
                flashlightPickupID, true);
        }

        ApplyPickupState();

        if (pickup != null && pickupSound != null)
            pickup.PlayOneShot(pickupSound);
    }

    void ApplyPickupState()
    {
        if (flashlight_table != null)
            flashlight_table.SetActive(!pickedUp);

        if (flashlight_player != null)
            flashlight_player.SetActive(pickedUp);

        if (inttext != null)
            inttext.SetActive(false);
    }
}
