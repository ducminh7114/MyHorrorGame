
using UnityEngine;
using TMPro;
using System.Collections;

public class lookAtObject : MonoBehaviour
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

    [Header("Key Save Settings")]
    public string keyID = "Key_01";

    [Header("Key Dialogue")]
    [TextArea]
    public string dialogue =
        "This is a key item. You can use it to unlock doors or chests.";

    public float dialogueTimer = 2f;

    private bool keyCollected = false;
    private Coroutine dialogueCoroutine;

    private void Start()
    {
        if (textOB != null)
            textOB.enabled = false;

        if (ObjName != null)
            ObjName.enabled = false;

        if (inttext != null)
            inttext.SetActive(false);

        interactable = false;

        if (isKey)
            RestoreKeyState();
    }

    private void Update()
    {
        if (isKey)
        {
            // If the real key is inactive, disable this interaction object.
            if (key != null && !key.activeInHierarchy)
            {
                DisableKeyInteraction();
                return;
            }

            if (keyCollected)
                return;
        }

        if (!interactable)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacted with " + gameObject.name);

            if (isKey)
                PickUpKey();
            else
                LookAtObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isKey && (keyCollected ||
            (key != null && !key.activeInHierarchy)))
        {
            DisableKeyInteraction();
            return;
        }

        if (!other.CompareTag("MainCamera"))
            return;

        if (inttext != null)
            inttext.SetActive(true);

        if (ObjName != null)
        {
            ObjName.text = objectName;
            ObjName.enabled = true;
        }

        if (!isKey && textOB != null)
            textOB.enabled = true;

        interactable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera"))
            return;

        HideInteractionUI();
        interactable = false;
    }

    private void LookAtObject()
    {
        Debug.Log("Looking at " + gameObject.name);

        if (inttext != null)
            inttext.SetActive(false);

        if (ObjName != null)
            ObjName.text = "";

        if (textOB != null)
        {
            textOB.text = description;
            textOB.enabled = true;
        }
    }

    private void PickUpKey()
    {
        if (keyCollected)
            return;

        keyCollected = true;
        interactable = false;

        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.SetObjectState(keyID, true);

        HideInteractionUI();

        if (textOB != null)
        {
            textOB.text = dialogue;
            textOB.enabled = true;
        }

        StartCoroutine(DeactivateAfterDialogue());
    }

    private IEnumerator DeactivateAfterDialogue()
    {
        // Keep the dialogue visible for its configured duration.
        yield return new WaitForSeconds(dialogueTimer);

        if (textOB != null)
            textOB.enabled = false;

        // Deactivate and save the actual key.
        if (key != null)
            SaveObjectState.SetActive(key, false);

        // Deactivate and save the interaction object.
        SaveObjectState.SetActive(gameObject, false);
    }

    private void RestoreKeyState()
    {
        if (GameSaveManager.Instance == null)
            return;

        if (GameSaveManager.Instance.TryGetObjectState(
            keyID, out bool collected) && collected)
        {
            keyCollected = true;
            DisableKeyInteraction();
            return;
        }

        // Also handle a key that was restored as inactive.
        if (key != null && !key.activeInHierarchy)
            DisableKeyInteraction();
    }

    private void DisableKeyInteraction()
    {
        keyCollected = true;
        interactable = false;

        HideInteractionUI();

        if (key != null && key.activeSelf)
            key.SetActive(false);

        // Disable only the interaction object, not its parent.
        gameObject.SetActive(false);
    }

    private void HideInteractionUI()
    {
        if (inttext != null)
            inttext.SetActive(false);

        if (ObjName != null)
        {
            ObjName.enabled = false;
            ObjName.text = "";
        }

        if (textOB != null)
        {
            textOB.enabled = false;
            textOB.text = "";
        }
    }

    private IEnumerator HideDialogue()
    {
        yield return new WaitForSeconds(dialogueTimer);

        if (textOB != null)
            textOB.enabled = false;

        dialogueCoroutine = null;
    }
}