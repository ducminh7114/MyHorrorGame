using UnityEngine;

public class padlockInteraction : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadUI;
    //public GameObject hud;
    //public GameObject inv;

    public GameObject inttext;
    //public AudioSource pickUpSound;
    public bool interactable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Trigger entered by: " + other.name);
            inttext.SetActive(true);
            interactable = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            keypadUI.SetActive(true);
            //hud.SetActive(false);
            //inv.SetActive(false);
            //pickUpSound.Play();
        }
    }
}
