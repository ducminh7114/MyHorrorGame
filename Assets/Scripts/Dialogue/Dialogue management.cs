using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Dialoguemanagement : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSegment
    {
        public string subjectName;

        [TextArea]
        public string dialogue;

        public bool skipable = true;

        [Range(1f, 25f)]
        public float lettersPerSecond = 20f;
    }

    [Header("UI")]
    public GameObject conversationPanel;
    public GameObject intText;
    private bool interactable;

    public TMP_Text subjectText;
    public TMP_Text bodyText;

    [Header("Dialogue")]
    public List<DialogueSegment> dialogues = new();

    [TextArea]
    public string repeatDialogue = ".";

    [Header("Camera")]
    public Transform lookTarget;
    public float lookDuration = 1f;
    public float zoomFOV = 40f;
    public float zoomOutTime = 1f;

    [Header("Player")]
    public playerMovement player;
    public PauseGame pause;
    public GameObject gunSystem;
    public Camera playerCamera;

    private bool playerInRange;
    private bool dialogueRunning;
    private bool hasRead;

    private bool isTyping;
    private bool skipTyping;

    private int dialogueIndex;

    private float originalFOV;

    void Start()
    {
        conversationPanel.SetActive(false);
        originalFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        if (interactable &&
    !dialogueRunning)
    if (Input.GetKeyDown(KeyCode.E))
        {
            intText.SetActive(false);
            StartCoroutine(StartDialogue());
        }

        if (!dialogueRunning)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                ShowNextDialogue();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = true;

            if (!dialogueRunning)
                intText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = false;
            intText.SetActive(false);
        }
    }


    IEnumerator StartDialogue()
    {
        dialogueRunning = true;

        player.canMove = false;
        player.canLook = false;
        pause.canPause = false;
        gunSystem.SetActive(false);


        Vector3 direction =
            lookTarget.position -
            playerCamera.transform.position;

        // Horizontal direction
        Vector3 horizontalDirection =
            new Vector3(direction.x, 0f, direction.z);

        Quaternion targetPlayerRotation =
            Quaternion.LookRotation(horizontalDirection);

        // Vertical direction
        float targetPitch =
            -Mathf.Atan2(
                direction.y,
                horizontalDirection.magnitude
            ) * Mathf.Rad2Deg;

        targetPitch = Mathf.Clamp(
            targetPitch,
            -player.lookXLimit,
            player.lookXLimit
        );

        Quaternion startPlayerRotation =
            player.transform.rotation;

        float startPitch =
            player.GetCameraPitch();

        float t = 0f;

        while (t < lookDuration)
        {
            t += Time.deltaTime;

            float progress =
                Mathf.Clamp01(t / lookDuration);

            // Horizontal
            player.transform.rotation =
                Quaternion.Slerp(
                    startPlayerRotation,
                    targetPlayerRotation,
                    progress
                );

            // Vertical
            float currentPitch =
                Mathf.Lerp(
                    startPitch,
                    targetPitch,
                    progress
                );

            player.SetCameraPitch(currentPitch);

            // Zoom
            playerCamera.fieldOfView =
                Mathf.Lerp(
                    originalFOV,
                    zoomFOV,
                    progress
                );

            yield return null;
        }

        // Exact final rotation
        player.transform.rotation =
            targetPlayerRotation;

        player.SetCameraPitch(targetPitch);

        conversationPanel.SetActive(true);

        if (hasRead)
        {
            bodyText.text = repeatDialogue;
            isTyping = false;
        }
        else
        {
            dialogueIndex = 0;
            StartCoroutine(
                TypeDialogue(dialogues[0])
            );
        }
    }

    IEnumerator TypeDialogue(DialogueSegment segment)
    {
        isTyping = true;
        skipTyping = false;

        bodyText.text = "";
        subjectText.text = segment.subjectName;

        float delay = 1f / segment.lettersPerSecond;

        foreach (char c in segment.dialogue)
        {
            if (skipTyping && segment.skipable)
            {
                bodyText.text = segment.dialogue;
                break;
            }

            bodyText.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    void ShowNextDialogue()
    {
        if (hasRead)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        dialogueIndex++;

        if (dialogueIndex >= dialogues.Count)
        {
            hasRead = true;
            StartCoroutine(EndDialogue());
            return;
        }

        StartCoroutine(
            TypeDialogue(
                dialogues[dialogueIndex]));
    }

IEnumerator EndDialogue()
{
    conversationPanel.SetActive(false);

    float t = 0f;

    while (t < zoomOutTime)
    {
        t += Time.deltaTime;

        playerCamera.fieldOfView =
            Mathf.Lerp(
                zoomFOV,
                originalFOV,
                t / zoomOutTime);

        yield return null;
    }

    player.canMove = true;
    player.canLook = true;
    pause.canPause = true;
    gunSystem.SetActive(true);

        if (interactable)
    {
        intText.SetActive(true);
    }

    dialogueRunning = false;
}
}

