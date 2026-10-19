
using System;
using System.Collections.Generic;

[Serializable]
public class WeaponInventorySaveData
{
    public string primaryWeapon;
    public string smallWeapon;
    public string meleeWeapon;
    public string equippedWeapon;

    public List<WeaponAmmoSaveData> ammo = new List<WeaponAmmoSaveData>();
}

[Serializable]
public class WeaponAmmoSaveData
{
    public string weaponName;
    public int magazineAmmo;
    public int reserveAmmo;
}
