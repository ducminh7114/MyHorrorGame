using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Holders")]
    public Transform primarySlot;
    public Transform smallSlot;
    public Transform meleeSlot;

    [Header("Current Equipped Weapon In Each Slot")]
    public GameObject currentPrimary;
    public GameObject currentSmall;
    public GameObject currentMelee;

    private GameObject currentEquipped;

    void Start()
    {
        // Hide every weapon first
        SetAllInactive(primarySlot);
        SetAllInactive(smallSlot);
        SetAllInactive(meleeSlot);

        // Equip starting weapons
        if (currentPrimary != null)
            currentPrimary.SetActive(true);

        if (currentSmall != null)
            currentSmall.SetActive(true);

        if (currentMelee != null)
            currentMelee.SetActive(true);

        // Start with the small weapon equipped
        EquipWeapon(currentSmall);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(currentPrimary);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(currentSmall);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipWeapon(currentMelee);
    }

    public void PickupWeapon(WeaponPickup pickup)
    {
        WeaponData data = pickup.weaponData;

        switch (data.weaponType)
        {
            case WeaponType.Primary:
                ReplaceWeapon(ref currentPrimary, data, primarySlot, pickup.transform.position);
                break;

            case WeaponType.Small:
                ReplaceWeapon(ref currentSmall, data, smallSlot, pickup.transform.position);
                break;

            case WeaponType.Melee:
                ReplaceWeapon(ref currentMelee, data, meleeSlot, pickup.transform.position);
                break;
        }

        Destroy(pickup.gameObject);
    }

    void ReplaceWeapon(ref GameObject currentWeapon,
                       WeaponData data,
                       Transform slot,
                       Vector3 dropPosition)
    {
        // Drop old weapon pickup
        if (currentWeapon != null)
        {
            WeaponDataReference oldData =
                currentWeapon.GetComponent<WeaponDataReference>();

            if (oldData != null &&
                oldData.weaponData.pickupPrefab != null)
            {
                Instantiate(oldData.weaponData.pickupPrefab,
                            dropPosition,
                            Quaternion.identity);
            }

            currentWeapon.SetActive(false);
        }

        // Find the weapon already under this slot
        WeaponDataReference[] weapons =
            slot.GetComponentsInChildren<WeaponDataReference>(true);

        foreach (WeaponDataReference weapon in weapons)
        {
            if (weapon.weaponData == data)
            {
                currentWeapon = weapon.gameObject;

                currentWeapon.SetActive(true);

                EquipWeapon(currentWeapon);

                return;
            }
        }

        Debug.LogWarning("Weapon not found under " + slot.name);
    }

    void EquipWeapon(GameObject weapon)
    {
        if (weapon == null)
            return;

        if (currentEquipped != null)
            currentEquipped.SetActive(false);

        currentEquipped = weapon;
        currentEquipped.SetActive(true);
    }

    void SetAllInactive(Transform slot)
    {
        foreach (Transform child in slot)
        {
            child.gameObject.SetActive(false);
        }
    }
}