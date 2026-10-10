using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;

    public GameObject pickupText;     // "Press E to Pick Up"

    private bool interactable = false;

    private WeaponInventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<WeaponInventory>();

        if (pickupText != null)
            pickupText.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = true;

            if (pickupText != null)
                pickupText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = false;

            if (pickupText != null)
                pickupText.SetActive(false);
        }
    }

    void Update()
    {
        if (!interactable)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.PickupWeapon(this);

            if (pickupText != null)
                pickupText.SetActive(false);

            Destroy(gameObject);
        }
    }
}   
