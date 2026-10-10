using UnityEngine;

public class lightCandle : MonoBehaviour
{
    public GameObject lightOB;
    public GameObject flame;
    public GameObject lightText;

    public bool unlit;
    public bool inReach;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlit = true;
        flame.SetActive(false);
        lightText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && unlit)
        {
            lightText.SetActive(true);
            inReach = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") && unlit)
        {
            lightText.SetActive(false);
            inReach = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        void Update()
        {
            if(lightOB.activeInHierarchy && inReach && Input.GetButtonDown("Interact"))
            {
                flame.SetActive(true);
                lightText.SetActive(false);
                unlit = false;
            }
        }
    }
}
