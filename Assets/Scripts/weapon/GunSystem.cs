using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
    
public class GunSystem : MonoBehaviour
{
    private WeaponDataReference weaponRef;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15;


    public GameObject fpsCam;                   //The Point Of Shooting
    public ParticleSystem muzzleFlash;          //Particle Effect For Muzzle Flash
    public GameObject impactEffect;             //Bullet Impact Effect
    public GameObject bulletCasing;             //Eject Used Casing
    public Transform casinglocation;            //Where The Casing Gets Ejected
    public AudioSource weaponSound;             //Weapon Sound Effect
    public AudioSource noAmmoSound;             //Empty Gun Sound 
    public AudioSource reloadSound;             //Reload Sound 

    public Animator anim;                       //Animations For Weapons
    public Vector3 reloading;                   //New Position For Reload
    public float reloadTime = 3;                //Time It Takes To Reload
    public Vector3 upRecoil;                    //New Position For Recoil
    Vector3 originalRotation;                   //Original Position

    public float amount;                        //Swaying Min Amount
    public float maxAmount;                     //Swaying Max Amount
    public float smoothAmount;                  //Smooth Time For Swaying

    private Vector3 initialPosition;            //Original Position Before Swaying

    public TMP_Text ammoText;                 //Ammo Text

    private int currentAmmo;                    //The Current Ammo In Weapon
    public int magazineSize = 10;               //How Much Ammo Is In Each Mag
    public int ammoCache = 20;                  //How Much Ammo Is In Your Cache (Storage)
    private int maxAmmo;                        //Max Ammo Is Private MaxAmmo = Mag Size
    private int ammoNeeded;                     //Ammo Counter For How Much Is Needed, You Shoot 5 Bullets, You Need 5

    public bool semi;                           //Is the Weapon Semi
    public bool auto;                           //Is The Weapon Auto
    public bool melee;                          //Is The Weapon Melee

    //There Can Be A Bug Where The Casing Goes Reversed, These Two Bools Will Fix It:

    public bool casingForward;                  //Get Correct Orientation Of Casings
    public bool casingBackwards;                //Get Correct Orientation Of Casings

    private bool isreloading;                   //Is The Weapon Reloading
    private bool canShoot;                      //Is The Weapon Able To Be Shot

    private float nextTimeToFire = 0f;          //How Much Time Must Pass Before Shooting/Meleeing Again



    //Start Function To Ensure Theres No Bugs:

    void Start()
    {
        weaponRef = GetComponent<WeaponDataReference>();

        if (weaponRef != null && weaponRef.weaponData != null)
        {
            // Load stats from WeaponData
            damage = weaponRef.weaponData.damage;
            range = weaponRef.weaponData.range;
            fireRate = weaponRef.weaponData.fireRate;

            // Load ammo
            magazineSize = weaponRef.weaponData.magazineCapacity;
            maxAmmo = magazineSize;
            ammoCache = weaponRef.weaponData.maxReserveAmmo;

            Debug.Log("Loaded Weapon Data: " + weaponRef.weaponData.weaponName);
        }
        else
        {
            Debug.LogWarning(gameObject.name + " has no WeaponDataReference or WeaponData assigned.");
        }

        isreloading = false;
        canShoot = true;

        originalRotation = transform.localEulerAngles;
        initialPosition = transform.localPosition;
    }



    void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        //For Semi Auto Weapons:

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && semi && magazineSize > 0 && canShoot)
        {
            AddRecoil();
            nextTimeToFire = Time.time + 1f / fireRate;
            anim.SetBool("shoot", true);
            Invoke("setboolback", .5f);
            Shoot();
        }

        //For Weapons With Melee:

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && melee)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            anim.SetInteger("AttackIndex", Random.Range(0, 3));
            anim.SetTrigger("Melee");
            Melee();
        }


        //For Auto Weapons:

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && magazineSize > 0 && auto && canShoot)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            anim.SetBool("shoot", true);
            Invoke("setboolback", .5f);
            AddRecoilAuto();
            Shoot();
        }

        //Checks For 0 Ammo:

        if (Input.GetButton("Fire1") && magazineSize == 0)
        {
            noAmmoSound.Play();
        }

        else if (Input.GetButtonUp("Fire1") && canShoot)
        {

            StopRecoil();
        }

        //Changeing Animation Weapon Ammo 0:

        if (magazineSize == 0)
        {
            ammoText.GetComponent<TMP_Text>().text = "Reload";
            anim.SetBool("empty", true);
        }

        if (magazineSize > 0)
        {
            anim.SetBool("empty", false);
        }


        //Reloading:

        if (Input.GetKeyDown(KeyCode.R) && magazineSize < maxAmmo && ammoCache > 0 && !isreloading)
        {
            canShoot = false;
            isreloading = true;

            int bulletsNeeded = maxAmmo - magazineSize;

            if (ammoCache >= bulletsNeeded)
            {
                ammoCache -= bulletsNeeded;
                magazineSize = maxAmmo;
            }
            else
            {
                magazineSize += ammoCache;
                ammoCache = 0;
            }

            StartCoroutine(ReloadTimer());
        }

        //Stops Bugs With Pressing Reload More Than Once:

        // Don't allow shooting while reloading
        if (isreloading)
        {
            ammoText.text = magazineSize + " / " + ammoCache;
            return;
        }

        //Doesnt Reload If Cache Is 0:

        if (Input.GetKeyDown(KeyCode.R) && ammoCache == 0)
        {

            return;

        }

        //Tells Our Text Object What To Say:

        ammoText.text = magazineSize + " / " + ammoCache;


        //Our Swaying Function Being Put To Action:

        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        //Making Sure The Sway Goes Back To Original Postion:

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);

    }


    //If Our Weapon Is A Gun:

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position,
                            fpsCam.transform.forward,
                            out hit,
                            range))
        {
            if (magazineSize <= 0)
                return;

            magazineSize--;

            WeaponInventory inventory = FindObjectOfType<WeaponInventory>();
            if (inventory != null)
                inventory.SaveInventory();

            ammoNeeded++;

            muzzleFlash.Play();
            weaponSound.Play();

            GameObject casing = Instantiate(bulletCasing, casinglocation.position, casinglocation.rotation);

            Destroy(casing, 2f);


            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            GameObject impactOB = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

            Destroy(impactOB, 2f);

            if (casingForward)
            {
                casing.GetComponent<Rigidbody>().AddForce(transform.forward * 250);
            }

            if (casingBackwards)
            {
                casing.GetComponent<Rigidbody>().AddForce(transform.forward * -250);
            }

        }
    }


    //If Our Weapon Is Melee:

    void Melee()
    {
        weaponSound.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

    }


    //Setting Animation Bools Back To Normal:

    public void setboolback()
    {
        anim.SetBool("shoot", false);
    }


    //Adding Recoil Postion When Shot Semi Weapons:

    public void AddRecoil()
    {
        if (canShoot)
        {
            transform.localEulerAngles += upRecoil;
            StartCoroutine(StopRecoilSemi());
        }
    }

    //Adding Recoil Postion When Shot Auto Weapons:

    public void AddRecoilAuto()
    {
        if (canShoot)
        {
            transform.localEulerAngles += upRecoil;
            StartCoroutine(StopRecoilSemi());
        }
    }

    //Stopping Recoil:

    public void StopRecoil()
    {
        transform.localEulerAngles = originalRotation;
    }

    //Stopping Recoil (Fixing Bugs)

    IEnumerator StopRecoilSemi()
    {
        yield return new WaitForSeconds(.1f);
        transform.localEulerAngles = originalRotation;
    }

    //Our Reload Timer:

    IEnumerator ReloadTimer()
    {
        reloadSound.Play();
        transform.localEulerAngles += reloading;
        yield return new WaitForSeconds(reloadTime);
        isreloading = false;
        canShoot = true;
        transform.localEulerAngles = originalRotation;
    }
}