using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class lookAtObjcet : MonoBehaviour
{
    [Header("Interaction")]
    public TMP_Text textOB;
    public GameObject inttext;
    public bool interactable;

    [Header("Object Information")]
    public string description = "description";
    public string objectName = "Item Name";
    public TMP_Text ObjName;

    [Header("Key Settings")]
    public bool isKey = false;
    public GameObject key;

    [Header("Key Dialogue")]
    [TextArea]
    public string dialogue = "This is a key item. You can use it to unlock doors or chests.";

    public float dialogueTimer = 2f;

    private Coroutine dialogueCoroutine;

    void Start()
    {
       if (textOB != null)
        {
            textOB.enabled = false;

        }

        if (ObjName != null)
        {
            ObjName.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(true);
            
            if (ObjName != null)
            {
                ObjName.text = objectName;
                ObjName.enabled = true;

            }
        
            if (!isKey && textOB != null)
            {
                textOB.enabled = true;
            }

            interactable = true;

        }

    }

    void OnTriggerExit(Collider Other)
    {
        if (Other.CompareTag("MainCamera"))
        {
            inttext.SetActive(false);

            if (textOB != null)
            {
                textOB.enabled = false;
                textOB.text = "";
            }

            if (objectName != null)
            {
                ObjName.enabled = false;
                ObjName.text = "";
            }

            interactable = false;

        }
    }

    void Update()
    {
        if (!interactable)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacted with " + gameObject.name);
            if(isKey)
            {
                PickUpKey();
            } else
            {
                LookAtObject();
            }
        }

    }

    void LookAtObject()
    {
        Debug.Log("Looking at " + gameObject.name);
        if (inttext != null)
            inttext.SetActive(false);

        if (ObjName != null)
            ObjName.text = "";

        if(textOB != null)
        {
            textOB.text = description;
            textOB.enabled = true;
        }
    }

    void PickUpKey()
    {
        Debug.Log("Picked up " + gameObject.name);
        if (inttext != null)
            inttext.SetActive(false);

        if (ObjName != null)
        {
            ObjName.text = "";
            ObjName.enabled = false;

        }

        if (textOB != null)
        {
            textOB.text = dialogue;
            textOB.enabled = true;
        }

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(HideDialogue());
    
        if (key != null)
        {
            key.SetActive(false);
        }

        interactable = false;

    }

    IEnumerator HideDialogue()
    {
        yield return new WaitForSeconds(dialogueTimer);
        if (textOB != null)
        {
            textOB.enabled = false;
        }

        dialogueCoroutine = null;
    }
}