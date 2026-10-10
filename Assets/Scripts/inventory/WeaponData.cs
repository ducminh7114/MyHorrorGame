using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    public WeaponType weaponType;

    public GameObject weaponPrefab;

    public GameObject pickupPrefab;

    public Sprite weaponIcon;

    [Header("Gun Stats")]
    public float damage;

    public float range;

    public float fireRate;

    public int magazineCapacity;

    public int maxReserveAmmo;
}
