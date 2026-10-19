using UnityEngine;
using System.Collections;
using TMPro;

public class PuzzlePickup : MonoBehaviour
{
    [Header("Puzzle Piece ID")]
    public int pieceID;

    [Header("Interaction UI")]
    public GameObject interactText;
    public TMP_Text textOB;
    public string description = "description";

    [Header("Puzzle Object")]
    public GameObject puzzlePiece;

    [Header("Save Settings")]
    [Tooltip("Unique ID for this pickup interaction.")]
    public string pickupID = "PuzzlePickup_01";

    [Header("Dialogue")]
    public float dialogueTimer = 2f;

    private bool canInteract;
    private bool collected;
    private Coroutine dialogueCoroutine;

    private void Start()
    {
        if (textOB != null)
            textOB.enabled = false;

        if (interactText != null)
            interactText.SetActive(false);

        canInteract = false;
        StartCoroutine(RestorePickupState());
    }

    private IEnumerator RestorePickupState()
{
    // Allow GameSaveManager time to restore saved scene/object states.
    yield return null;
    yield return null;
    yield return new WaitForSecondsRealtime(0.2f);

    if (GameSaveManager.Instance != null &&
        !string.IsNullOrEmpty(pickupID) &&
        GameSaveManager.Instance.TryGetObjectState(pickupID, out bool wasCollected) &&
        wasCollected)
    {
        collected = true;
        canInteract = false;
        HideInteractionUI();

        if (puzzlePiece != null && puzzlePiece.activeSelf)
            puzzlePiece.SetActive(false);

        gameObject.SetActive(false);
    }
}

private void Update()
{
    if (collected || !canInteract)
        return;

    if (Input.GetKeyDown(KeyCode.E))
        Pickup();
}

private void Pickup()
{
    if (collected || PuzzleManager.Instance == null)
        return;

    collected = true;
    canInteract = false;
    PuzzleManager.Instance.CollectPiece(pieceID);

    if (GameSaveManager.Instance != null && !string.IsNullOrEmpty(pickupID))
        GameSaveManager.Instance.SetObjectState(pickupID, true);

    HideInteractionUI();

    if (textOB != null)
    {
        textOB.text = description;
        textOB.enabled = true;
    }

    if (dialogueCoroutine != null)
        StopCoroutine(dialogueCoroutine);

    dialogueCoroutine = StartCoroutine(HideDialogueAfterDelay());

    if (puzzlePiece != null)
        SaveObjectState.SetActive(puzzlePiece, false);

    StartCoroutine(DeactivatePickupAfterDialogue());
}

private IEnumerator DeactivatePickupAfterDialogue()
{
    yield return new WaitForSeconds(dialogueTimer);

    HideInteractionUI();
    SaveObjectState.SetActive(gameObject, false);
}

private void HideInteractionUI()
{
    if (interactText != null)
        interactText.SetActive(false);

    if (textOB != null)
    {
        textOB.enabled = false;
        textOB.text = "";
    }
}

private void OnTriggerEnter(Collider other)
{
    if (collected || !other.CompareTag("MainCamera"))
        return;

    canInteract = true;

    if (interactText != null)
        interactText.SetActive(true);
}

private void OnTriggerExit(Collider other)
{
    if (!other.CompareTag("MainCamera"))
        return;

    canInteract = false;

    if (!collected)
        HideInteractionUI();
}

private IEnumerator HideDialogueAfterDelay()
{
    yield return new WaitForSeconds(dialogueTimer);

    if (textOB != null)
    {
        textOB.enabled = false;
        textOB.text = "";
    }

    dialogueCoroutine = null;
}
}