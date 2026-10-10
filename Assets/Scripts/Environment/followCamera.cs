using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class followCamera : MonoBehaviour
{
    [SerializeField] Transform PlayerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerCamera);
    }
}
