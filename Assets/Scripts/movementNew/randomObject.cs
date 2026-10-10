using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class randomObject : MonoBehaviour
{
    public GameObject[] items;
    public int numberToEnable = 1;

    void Start()
    {
        RandomizeActive();
    }

    public void RandomizeActive()
    {
        numberToEnable = Mathf.Clamp(numberToEnable, 0, items.Length);

        foreach (GameObject item in items)
        {
            item.SetActive(false);
        }
        
        List<int> indexes = new List<int>(); // List to hold selected indexes
        for (int i = 0; i < items.Length; i++)
        { 
            indexes.Add(i); 
        }

        for (int i = 0; i < numberToEnable; i++)
        {
            int rand = Random.Range(0, indexes.Count);
            items[indexes[rand]].SetActive(true);
            indexes.RemoveAt(rand);
        }
    }
}
