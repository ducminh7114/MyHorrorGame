
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Holders")]
    public Transform primarySlot;
    public Transform smallSlot;
    public Transform meleeSlot;

    [Header("Current Weapons")]
    public GameObject currentPrimary;
    public GameObject currentSmall;
    public GameObject currentMelee;

    private GameObject currentEquipped;
    private bool ready;

    IEnumerator Start()
    {
        SetAllInactive(primarySlot);
        SetAllInactive(smallSlot);
        SetAllInactive(meleeSlot);

        // Wait for the save manager and scene objects to initialize.
        yield return null;
        yield return null;

        RestoreInventory();
        ready = true;
    }

    void Update()
    {
        if (!ready)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(currentPrimary);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(currentSmall);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipWeapon(currentMelee);
    }

    public void PickupWeapon(WeaponPickup pickup)
    {
        if (pickup == null || pickup.weaponData == null)
            return;

        WeaponData data = pickup.weaponData;

        GameSaveManager manager = GameSaveManager.Instance;
        if (manager != null && !string.IsNullOrEmpty(pickup.pickupID))
            manager.MarkWeaponPickupCollected(pickup.pickupID);

        switch (data.weaponType)
        {
            case WeaponType.Primary:
                ReplaceWeapon(ref currentPrimary, data, primarySlot,
                    pickup.transform.position);
                break;

            case WeaponType.Small:
                ReplaceWeapon(ref currentSmall, data, smallSlot,
                    pickup.transform.position);
                break;

            case WeaponType.Melee:
                ReplaceWeapon(ref currentMelee, data, meleeSlot,
                    pickup.transform.position);
                break;
        }

        SaveInventory();

        Destroy(pickup.gameObject);
    }

    void ReplaceWeapon(
        ref GameObject currentWeapon,
        WeaponData data,
        Transform slot,
        Vector3 dropPosition)
    {
        if (slot == null)
        {
            Debug.LogWarning("WeaponInventory: A weapon slot is not assigned.");
            return;
        }

        if (currentWeapon != null && currentWeapon != currentEquipped)
        {
            WeaponDataReference oldData =
                currentWeapon.GetComponent<WeaponDataReference>();

            if (oldData != null && oldData.weaponData != null &&
                oldData.weaponData.pickupPrefab != null)
            {
                Instantiate(oldData.weaponData.pickupPrefab,
                    dropPosition, Quaternion.identity);
            }

            currentWeapon.SetActive(false);
        }
        else if (currentWeapon != null)
        {
            WeaponDataReference oldData =
                currentWeapon.GetComponent<WeaponDataReference>();

            if (oldData != null && oldData.weaponData != null &&
                oldData.weaponData.pickupPrefab != null)
            {
                Instantiate(oldData.weaponData.pickupPrefab,
                    dropPosition, Quaternion.identity);
            }

            currentWeapon.SetActive(false);
            currentEquipped = null;
        }

        WeaponDataReference[] weapons =
            slot.GetComponentsInChildren<WeaponDataReference>(true);

        foreach (WeaponDataReference weapon in weapons)
        {
            if (weapon.weaponData == data)
            {
                currentWeapon = weapon.gameObject;
                EquipWeapon(currentWeapon);
                return;
            }
        }

        Debug.LogWarning("Weapon not found under slot: " + slot.name);
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (weapon == null)
            return;

        if (currentEquipped != null)
            currentEquipped.SetActive(false);

        currentEquipped = weapon;
        currentEquipped.SetActive(true);

        SaveInventory();
    }

    public void SaveInventory()
    {
        if (GameSaveManager.Instance == null)
            return;

        WeaponInventorySaveData data = new WeaponInventorySaveData();

        data.primaryWeapon = GetWeaponName(currentPrimary);
        data.smallWeapon = GetWeaponName(currentSmall);
        data.meleeWeapon = GetWeaponName(currentMelee);
        data.equippedWeapon = GetWeaponName(currentEquipped);

        HashSet<string> savedWeapons = new HashSet<string>();

        WeaponDataReference[] weapons = GetComponentsInChildren<WeaponDataReference>(true);

        foreach (WeaponDataReference reference in weapons)
            AddAmmoData(reference, data, savedWeapons);

        AddAmmoFromSlot(primarySlot, data, savedWeapons);
        AddAmmoFromSlot(smallSlot, data, savedWeapons);
        AddAmmoFromSlot(meleeSlot, data, savedWeapons);

        GameSaveManager.Instance.SaveWeaponInventory(data);
    }

    void AddAmmoFromSlot(
        Transform slot,
        WeaponInventorySaveData data,
        HashSet<string> savedWeapons)
    {
        if (slot == null)
            return;

        foreach (WeaponDataReference reference in
                 slot.GetComponentsInChildren<WeaponDataReference>(true))
        {
            AddAmmoData(reference, data, savedWeapons);
        }
    }

    void AddAmmoData(
        WeaponDataReference reference,
        WeaponInventorySaveData data,
        HashSet<string> savedWeapons)
    {
        if (reference == null || reference.weaponData == null)
            return;

        string name = reference.weaponData.weaponName;

        if (string.IsNullOrEmpty(name) || !savedWeapons.Add(name))
            return;

        GunSystem gun = reference.GetComponent<GunSystem>();
        if (gun == null)
            return;

        data.ammo.Add(new WeaponAmmoSaveData
        {
            weaponName = name,
            magazineAmmo = gun.magazineSize,
            reserveAmmo = gun.ammoCache
        });
    }

    void RestoreInventory()
    {
        if (GameSaveManager.Instance == null)
        {
            EquipWeapon(currentSmall);
            return;
        }

        WeaponInventorySaveData data =
            GameSaveManager.Instance.GetWeaponInventory();

        currentPrimary = FindWeapon(data.primaryWeapon);
        currentSmall = FindWeapon(data.smallWeapon);
        currentMelee = FindWeapon(data.meleeWeapon);

        // Preserve configured starting weapons if a saved name cannot be found.
        if (currentPrimary == null && string.IsNullOrEmpty(data.primaryWeapon))
            currentPrimary = FindDefaultWeapon(primarySlot);

        if (currentSmall == null && string.IsNullOrEmpty(data.smallWeapon))
            currentSmall = FindDefaultWeapon(smallSlot);

        if (currentMelee == null && string.IsNullOrEmpty(data.meleeWeapon))
            currentMelee = FindDefaultWeapon(meleeSlot);

        RestoreAmmo(data);

        currentEquipped = null;
        GameObject equipped = FindWeapon(data.equippedWeapon);

        if (equipped == null)
            equipped = currentSmall;

        EquipWeaponWithoutSaving(equipped);
    }

    void RestoreAmmo(WeaponInventorySaveData data)
    {
        if (data.ammo == null)
            return;

        foreach (WeaponAmmoSaveData ammo in data.ammo)
        {
            GameObject weapon = FindWeapon(ammo.weaponName);
            if (weapon == null)
                continue;

            GunSystem gun = weapon.GetComponent<GunSystem>();
            if (gun == null)
                continue;

            gun.magazineSize = ammo.magazineAmmo;
            gun.ammoCache = ammo.reserveAmmo;
        }
    }

    GameObject FindWeapon(string weaponName)
    {
        if (string.IsNullOrEmpty(weaponName))
            return null;

        Transform[] slots = { primarySlot, smallSlot, meleeSlot };

        foreach (Transform slot in slots)
        {
            if (slot == null)
                continue;

            foreach (WeaponDataReference reference in
                     slot.GetComponentsInChildren<WeaponDataReference>(true))
            {
                if (reference.weaponData != null &&
                    reference.weaponData.weaponName == weaponName)
                    return reference.gameObject;
            }
        }

        return null;
    }

    GameObject FindDefaultWeapon(Transform slot)
    {
        if (slot == null)
            return null;

        foreach (WeaponDataReference reference in
                 slot.GetComponentsInChildren<WeaponDataReference>(true))
        {
            if (reference.weaponData != null)
                return reference.gameObject;
        }

        return null;
    }

    string GetWeaponName(GameObject weapon)
    {
        if (weapon == null)
            return "";

        WeaponDataReference reference =
            weapon.GetComponent<WeaponDataReference>();

        return reference != null && reference.weaponData != null
            ? reference.weaponData.weaponName
            : "";
    }

    void EquipWeaponWithoutSaving(GameObject weapon)
    {
        SetAllInactive(primarySlot);
        SetAllInactive(smallSlot);
        SetAllInactive(meleeSlot);

        currentEquipped = weapon;

        if (currentEquipped != null)
            currentEquipped.SetActive(true);
    }

    void SetAllInactive(Transform slot)
    {
        if (slot == null)
            return;

        foreach (Transform child in slot)
            child.gameObject.SetActive(false);
    }
}
