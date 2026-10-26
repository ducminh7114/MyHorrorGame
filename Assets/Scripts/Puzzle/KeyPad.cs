using UnityEngine;
using System.Collections;
using TMPro;

public class KeyPad : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public GameObject keypadOB;
    public playerMovement playerMove;

    [Header("Lock")]
    public GameObject animatedOB;
    public Animator ANI;
    public padlockInteraction padlockScript;
    public TMP_Text textOB;

    [HideInInspector]
    public string answer;

    private bool isKeypadActive = false;
    private bool isOpening = false;

    public PuzzleManager puzzleManager;
    public bool Animate = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Range(0f, 1f)]
    public float audioVolume = 0.8f;

    void Start()
    {
        if (player != null)
        {
            playerMove = player.GetComponent<playerMovement>();
        }

        if (keypadOB != null)
        {
            keypadOB.SetActive(false);
        }

        UpdateAnswer();
    }

    void UpdateAnswer()
    {
        if (puzzleManager != null)
        {
            answer = puzzleManager.generatedCode;
        }

        if (string.IsNullOrEmpty(answer))
        {
            answer = PlayerPrefs.GetString("KeypadCode", "");
        }

        Debug.Log("Keypad answer: " + answer);
    }

    // Play a sound without interrupting other keypad sounds.
    void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, audioVolume);
    }

    public void Number(int number)
    {
        if (textOB == null || isOpening)
            return;

        PlaySound(buttonSound);

        textOB.text += number.ToString();
    }

    public void Execute()
    {
        if (textOB == null || isOpening)
            return;

        if (string.IsNullOrEmpty(answer))
        {
            UpdateAnswer();
        }

        if (!string.IsNullOrEmpty(answer) && textOB.text == answer)
        {
            // Correct code
            PlaySound(correctSound);

            textOB.text = "Right";
            isOpening = true;

            if (ANI != null)
            {
                ANI.SetTrigger("open");
            }

            Debug.Log("Keypad correct! Lock opened.");

            if (padlockScript != null)
            {
                padlockScript.enabled = false;
            }

            StartCoroutine(CloseKeypadAfterDelay(1f));
        }
        else
        {
            // Incorrect code
            PlaySound(wrongSound);

            textOB.text = "";
        }
    }

    public void Clear()
    {
        if (textOB == null || isOpening)
            return;

        PlaySound(buttonSound);

        textOB.text = "";
    }

    public void Exit()
    {
        CloseKeypad();
    }

    public void Update()
    {
        if (keypadOB != null && keypadOB.activeInHierarchy)
        {
            isKeypadActive = true;

            if (playerMove != null)
            {
                playerMove.enabled = false;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            isKeypadActive = false;
        }
    }

    private IEnumerator CloseKeypadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        CloseKeypad();
    }

    private void CloseKeypad()
    {
        if (keypadOB != null)
        {
            keypadOB.SetActive(false);
        }

        if (playerMove != null)
        {
            playerMove.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isKeypadActive = false;
        isOpening = false;
    }
}