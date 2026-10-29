
using UnityEngine;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public TMP_Text textOB;
    public GameObject Activator;
    [TextArea]
    public string dialogue = "Dialogue";
    public float timer = 2f;

    [Header("Save System")]
    public string triggerID = "Dialogue_01";
    public bool triggerOnce = true;

    private bool hasTriggered;
    private Coroutine dialogueCoroutine;

    void Start()
    {
        if (textOB != null)
            textOB.enabled = false;

        if (triggerOnce &&
            GameSaveManager.Instance != null &&
            GameSaveManager.Instance.IsTriggerCompleted(triggerID))
        {
            hasTriggered = true;

            if (Activator != null)
                Destroy(Activator);

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to the actual Player-tagged collider.
        if (!other.CompareTag("Player"))
            return;

        if (hasTriggered)
            return;

        hasTriggered = true;

        // Save event completion, NOT the player's position.
        if (triggerOnce && GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.MarkTriggerCompleted(triggerID);
        }

        if (textOB != null)
        {
            textOB.text = dialogue;
            textOB.enabled = true;
        }

        dialogueCoroutine = StartCoroutine(DisableText());
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);

        if (textOB != null)
            textOB.enabled = false;

        if (Activator != null)
            Destroy(Activator);

        if (triggerOnce)
        {
            gameObject.SetActive(false);
        }
        else
        {
            hasTriggered = false;
            dialogueCoroutine = null;
        }
    }

    void OnDisable()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        if (textOB != null)
            textOB.enabled = false;
    }
}

