using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class randomObject1 : MonoBehaviour
{
    public GameObject drop1;
    public GameObject drop2;
    public GameObject drop3;

    public int randNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randNum = Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (randNum == 0)
        {
            drop1.SetActive(true);
        }
        if (randNum == 1)
        {
            drop2.SetActive(true);
        }
        if (randNum == 2)
        {
            drop3.SetActive(true);
        }
    }
}