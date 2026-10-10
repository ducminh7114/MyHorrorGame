using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour
{
    public GameObject theAmmo;
    public GameObject weaponOB;
    public GameObject pickUpText;

    public AudioSource pickUpSound;

    public int ammoBoxAmount;

    public bool interactable;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            pickUpText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            pickUpText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                weaponOB.GetComponent<GunSystem>().ammoCache += ammoBoxAmount;

                pickUpText.SetActive(false);
                interactable = false;

                if (pickUpSound != null)
                {
                    pickUpSound.Play();
                }

                theAmmo.SetActive(false);
            }
        }
    }
}