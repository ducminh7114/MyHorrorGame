using UnityEngine;
using System.Collections;
using TMPro;

public class KeyPad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadOB;
    public playerMovement playerMove;
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

    void Start()
    {
        // Get player movement
        if (player != null)
        {
            playerMove = player.GetComponent<playerMovement>();
        }

        // Hide keypad at start
        if (keypadOB != null)
        {
            keypadOB.SetActive(false);
        }

        // Get the generated puzzle code
        UpdateAnswer();
    }

    void UpdateAnswer()
    {
        if (puzzleManager != null)
        {
            answer = puzzleManager.generatedCode;
        }

        // If it hasn't been loaded yet, try PlayerPrefs
        if (string.IsNullOrEmpty(answer))
        {
            answer = PlayerPrefs.GetString("KeypadCode", "");
        }

        Debug.Log("Keypad answer: " + answer);
    }

    public void Number(int number)
    {
        if (textOB == null)
            return;

        // Don't allow numbers after keypad is solved
        if (isOpening)
            return;

        textOB.text += number.ToString();
    }

    public void Execute()
    {
        if (textOB == null)
            return;

        if (string.IsNullOrEmpty(answer))
        {
            UpdateAnswer();
        }

        if (textOB.text == answer && !string.IsNullOrEmpty(answer))
        {
            // Correct
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
            // Wrong
            textOB.text = "Wrong";

            // Keep your original behavior
            textOB.text = "";
        }
    }

    public void Clear()
    {
        if (textOB != null)
        {
            textOB.text = "";
        }
    }

    public void Exit()
    {
        CloseKeypad();
    }

    public void Update()
    {
        // If keypad is open, disable player movement
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