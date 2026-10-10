using UnityEngine;

public class jumpScareTrigger : MonoBehaviour
{
    public GameObject scareObject;
    public AudioSource jumpscareSound;
    public Collider triggerCollider;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            scareObject.SetActive(true);
            //jumpscareSound.Play();
            triggerCollider.enabled = false;
        }
    }
}
