using TMPro;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class DoorBell : MonoBehaviour
{
    [Header("UI")]
    public GameObject inttext;
    public TMP_Text ObjName;

    [Header("Toggle")]
    public AudioSource toggleSound;

    [Header("Object information")]
    public string objectName = "Item Name";
    public bool interactable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inttext != null)
            inttext.SetActive(false);
        if (ObjName != null)
            ObjName.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("MainCamera"))
        {
            if(!interactable)
            {
                interactable = true;
            }
            if(inttext != null)
            {
                inttext.SetActive(true);
            }
            if (ObjName != null)
            {
                ObjName.text = objectName;
                ObjName.enabled = true;
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (!interactable)
            {
                interactable = true;
            }
            if (inttext != null)
            {
                inttext.SetActive(true);
            }
            if (ObjName != null)
            {
                ObjName.text = objectName;
                ObjName.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Door bell pressed");
            toggleSound.Play();
        }
     }
}
