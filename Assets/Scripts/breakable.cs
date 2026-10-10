using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public Rigidbody rb;
    public List<string> breakTags = new List<string>();

    private bool isBroken;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBroken) return;

        if (breakTags.Contains(other.tag))
        {
            isBroken = true;
            rb.isKinematic = false;
        }
    }
}
