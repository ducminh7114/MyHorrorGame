using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class moveCamera  : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
