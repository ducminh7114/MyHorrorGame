using UnityEngine;

public class lighter : MonoBehaviour
{
    public GameObject Lighter;
    public GameObject flames;

    public AudioSource lighterSound;

    public bool isOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOn = false;
        flames.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("fire1") && Lighter.activeInHierarchy)
        {
            flames.SetActive(true);
            lighterSound.Play();
            isOn = true;
        }

        else if (Input.GetButtonDown("fire1") && isOn)
        {
            return;
        }

        if (Input.GetButtonUp("fire2") && Lighter.activeInHierarchy && isOn)
        {
            flames.SetActive(false);
            isOn = false;
        }
    }
}
