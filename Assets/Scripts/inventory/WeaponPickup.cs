
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;
    public GameObject pickupText;

    private bool interactable = false;
    private WeaponInventory inventory;

    [Header("Save System")]
    public string pickupID;

    void Start()
    {
        inventory = FindObjectOfType<WeaponInventory>();

        if (pickupText != null)
            pickupText.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
            return;

        interactable = true;

        if (pickupText != null)
            pickupText.SetActive(true);

        if (GameSaveManager.Instance != null &&
    !string.IsNullOrEmpty(pickupID) &&
    GameSaveManager.Instance.WasWeaponPickupCollected(pickupID))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
            return;

        interactable = false;

        if (pickupText != null)
            pickupText.SetActive(false);
    }

    void Update()
    {
        if (!interactable || !Input.GetKeyDown(KeyCode.E))
            return;

        if (inventory == null)
        {
            Debug.LogWarning("WeaponPickup: WeaponInventory not found.");
            return;
        }

        if (weaponData == null)
        {
            Debug.LogWarning("WeaponPickup: WeaponData is not assigned.");
            return;
        }

        inventory.PickupWeapon(this);

        if (pickupText != null)
            pickupText.SetActive(false);
    }
}
