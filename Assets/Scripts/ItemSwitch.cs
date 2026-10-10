using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemSwitch : MonoBehaviour
{
    public GameObject Object1;
    public GameObject Object2;
    public GameObject Object3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Object1.SetActive(false);
        Object2.SetActive(false);
        Object3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("1"))
        {
            Object1.SetActive(true);
            Object2.SetActive(false);
            Object3.SetActive(false);
        }
        if (Input.GetButtonDown("2"))
        {
            Object1.SetActive(false);
            Object2.SetActive(true);
            Object3.SetActive(false);
        }
        if (Input.GetButtonDown("3"))
        {
            Object1.SetActive(false);
            Object2.SetActive(false);
            Object3.SetActive(true);
        }
    }
}


