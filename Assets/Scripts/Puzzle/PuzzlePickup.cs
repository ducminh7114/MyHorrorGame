using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PuzzlePickup : MonoBehaviour
{
    [Header("Puzzle Piece ID")]
    public int pieceID;

    [Header("Interaction UI")]
    public GameObject interactText;
    public TMP_Text textOB;
    public string description = "description";

    private bool canInteract;

    public GameObject puzzlePiece;

    public float dialogueTimer = 2f;

    private Coroutine dialogueCoroutine;

    private void Start()
    {
        if (textOB != null)
            textOB.enabled = false;

        if (interactText != null)
            interactText.SetActive(false);

    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        Debug.Log("Picked up " + gameObject.name);
        if (PuzzleManager.Instance == null)
            return;

        PuzzleManager.Instance.CollectPiece(pieceID);

        if (interactText != null)
            interactText.SetActive(false);

        if (textOB != null)
            {
                textOB.text = description;
                textOB.enabled = true;
            }

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(HideDialogueAfterDelay());

        if (puzzlePiece != null)
        {
            puzzlePiece.SetActive(false);
        }
        canInteract = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            canInteract = true;

            if (interactText != null)
            {
                interactText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            canInteract = false;

            if (interactText != null)
                interactText.SetActive(false);

            if (textOB != null)
            {
                textOB.enabled = false;
                textOB.text = "";
            }
        }
    }

    IEnumerator HideDialogueAfterDelay()
    {
        yield return new WaitForSeconds(dialogueTimer);
        if (textOB != null)
            { textOB.enabled = false; }
        dialogueCoroutine = null;
    }
}