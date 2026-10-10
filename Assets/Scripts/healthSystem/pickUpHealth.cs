using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class pickUpHealth : MonoBehaviour
{
    public GameObject pickUpOB;
    public GameObject player;
    public GameObject pickupText;
    public GameObject cannotPickupText;
    public float addHealth = 25f;
    private float currentHealth;

    public AudioSource healthPickupSound;
    public GameObject screenFX;

    public bool interactable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = true;
            pickupText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = false;
            pickupText.SetActive(false);
            cannotPickupText.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = player.GetComponent<playerHealth>().Health;
        cannotPickupText.SetActive(false);
        pickupText.SetActive(false);

        screenFX.SetActive(false);

        interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(interactable && Input.GetKeyDown(KeyCode.E) && player.GetComponent<playerHealth>().Health < 100)
        {
            interactable = false;
            healthPickupSound.Play();
            player.GetComponent<playerHealth>().Health += addHealth;
            screenFX.SetActive(true);
            pickUpOB.GetComponent<MeshRenderer>().enabled = false;
            pickUpOB.GetComponent<Collider>().enabled = false;
            pickupText.SetActive(false);
            StartCoroutine(TurnScreenFXOFF());
        }

        else if (interactable && Input.GetKeyDown(KeyCode.E) && player.GetComponent<playerHealth>().Health >= 100)
        {
            pickupText.SetActive(false);
            cannotPickupText.SetActive(true);
        }
    }
    IEnumerator TurnScreenFXOFF()
    {
        yield return new WaitForSeconds(1.5f);
        screenFX.SetActive(false);
        pickUpOB.SetActive(false);
    }
}
