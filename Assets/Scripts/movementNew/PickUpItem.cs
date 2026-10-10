using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public GameObject inttext;
    public GameObject flashlight_table;
    public GameObject flashlight_player;
    public AudioSource pickup;
    public bool interactable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
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

    void Update ()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            flashlight_player.SetActive(true);
            flashlight_table.SetActive(false);
            //pickup.Play();
            inttext.SetActive(false);
            interactable = false;
        }
    }
}

