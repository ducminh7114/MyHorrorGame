using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class readNote : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject inttext;
    public bool interactable;
    public PauseGame pause;

    [Header("Note Represent")]
    public GameObject noteRepresent;

    [Header("Note UI")]
    public GameObject noteUI;
    public TMP_Text ObjName;
    public TMP_Text noteText;

    [Header("Note Information")]
    public string noteTitle = "Note";

    [TextArea(10, 20)]
    public string noteContent = "This is the content of the note.";

    [Header("Player")]
    public GameObject player;

    [Header("Objects To Disable While Reading")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("Sound")]
    public AudioSource toggleSound;


    private bool readingNote = false;

    void Start()
    {
        if (noteUI != null)
            noteUI.SetActive(false);

        if (noteRepresent != null)
            noteRepresent.SetActive(false);

        if (inttext != null)
            inttext.SetActive(false);

        if (noteText != null)
            noteText.text = "";

        if (ObjName != null)
            ObjName.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Trigger entered by: " + other.name);

            if (!readingNote)
            {
                if (inttext != null)
                    inttext.SetActive(true);

                if (ObjName != null)
                {
                    ObjName.text = noteTitle;
                    ObjName.enabled = true;
                }

                interactable = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (inttext != null)
                inttext.SetActive(false);

            if (ObjName != null)
            {
                ObjName.text = "";
                ObjName.enabled = false;
            }

            interactable = false;
        }
    }

    void Update()
    {
        // If currently reading the note,
        // ESC should ONLY close the note.
        if (readingNote)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseNote();
            }

            return;
        }

        // Open note with E
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
    }

    void OpenNote()
    {
        Debug.Log("Reading note: " + gameObject.name);

        readingNote = true;
        interactable = false;
        toggleSound.Play();

        // Stop PauseGame from responding to ESC
        if (pause != null)
            pause.canPause = false;

        if (noteRepresent != null)
            noteRepresent.SetActive(true);

        if (inttext != null)
            inttext.SetActive(false);

        if (ObjName != null)
            ObjName.text = "";

        if (noteUI != null)
            noteUI.SetActive(true);

        if (noteText != null)
            noteText.text = noteContent;

        // Disable player movement
        if (player != null)
        {
            playerMovement movement = player.GetComponent<playerMovement>();

            if (movement != null)
                movement.enabled = false;
        }

        // Disable selected objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseNote()
    {
        Debug.Log("Stopped reading note.");

        readingNote = false;
        toggleSound.Play();

        if (noteUI != null)
            noteUI.SetActive(false);

        if (noteText != null)
            noteText.text = "";

        // Enable player movement
        if (player != null)
        {
            playerMovement movement = player.GetComponent<playerMovement>();

            if (movement != null)
                movement.enabled = true;
        }

        // Enable selected objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (noteRepresent != null)
            noteRepresent.SetActive(false);

        // Allow PauseGame to use ESC again
        if (pause != null)
            pause.canPause = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}