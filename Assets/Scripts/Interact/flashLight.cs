using UnityEngine;

public class flashLight : MonoBehaviour
{
    public GameObject light;
    public bool toggle;
    public AudioSource toggleSound;

    void Start()
    {
        if (toggle == true)
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }
    void Update()
    {
        toggle = !toggle;
        if (Input.GetKeyDown(KeyCode.F))
        {
            //toggleSound.Play();
            light.SetActive(false);
            if (toggle == true)
            {
                light.SetActive(true);
            }
        }
    }
}
