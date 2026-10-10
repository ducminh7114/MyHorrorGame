using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Puzzle Pieces On Table")]
    public GameObject[] puzzlePieces;

    [Header("Completion UI")]
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;

    [Header("Final Word")]
    public string finalWord = "BASEMENT";

    [Header("Generated Keypad Code")]
    public string generatedCode;

    [Header("Completed Paper")]
    public GameObject paper;

    [Header("Paper Read Note")]
    public readNote paperReadNote;

    private bool[] collectedPieces;

    private void Awake()
    {
        Instance = this;

        // TEMPORARY - DELETE THIS AFTER TESTING
        PlayerPrefs.DeleteAll();

        collectedPieces = new bool[puzzlePieces.Length];
    }

    private void Start()
    {
        generatedCode = PlayerPrefs.GetString("KeypadCode", "");

        // Hide old completion UI
        if (completionPanel != null)
            completionPanel.SetActive(false);

        // Hide physical paper
        if (paper != null)
            paper.SetActive(false);

        LoadPuzzle();
    }

    public void CollectPiece(int pieceID)
    {
        if (pieceID < 0 || pieceID >= puzzlePieces.Length)
            return;

        if (collectedPieces[pieceID])
            return;

        collectedPieces[pieceID] = true;

        // Show the corresponding piece on the table
        if (puzzlePieces[pieceID] != null)
            puzzlePieces[pieceID].SetActive(true);

        // Save progress
        PlayerPrefs.SetInt("PuzzlePiece_" + pieceID, 1);
        PlayerPrefs.Save();

        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        for (int i = 0; i < collectedPieces.Length; i++)
        {
            if (!collectedPieces[i])
            {
                return;
            }
        }

        PuzzleCompleted();
    }

    private void PuzzleCompleted()
    {
        // Generate the code only if one doesn't already exist
        if (string.IsNullOrEmpty(generatedCode))
        {
            generatedCode = Random.Range(10000, 100000).ToString();

            PlayerPrefs.SetString("KeypadCode", generatedCode);
            PlayerPrefs.Save();
        }

        Debug.Log("Puzzle completed!");
        Debug.Log("Generated keypad code: " + generatedCode);

        // Don't show the old completion UI
        if (completionPanel != null)
            completionPanel.SetActive(false);

        // Show the physical paper
        if (paper != null)
            paper.SetActive(true);

        // Put the generated code into the existing readNote
        if (paperReadNote != null)
        {
            paperReadNote.noteTitle = "Keypad Code";

            paperReadNote.noteContent =
                "The keypad code is:\n\n" +
                generatedCode;
        }
    }

    private void LoadPuzzle()
    {
        bool allCollected = true;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            bool collected =
                PlayerPrefs.GetInt("PuzzlePiece_" + i, 0) == 1;

            collectedPieces[i] = collected;

            if (puzzlePieces[i] != null)
                puzzlePieces[i].SetActive(collected);

            if (!collected)
                allCollected = false;
        }

        // If everything was already collected,
        // show the paper again.
        if (allCollected)
        {
            PuzzleCompleted();
        }
    }
}