
using UnityEngine;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public TMP_Text textOB;
    public GameObject Activator;
    public string dialogue = "Dialogue";
    public float timer = 2f;

    [Header("Save System")]
    public string triggerID = "Dialogue_01";
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void Start()
    {
        if (textOB != null)
            textOB.enabled = false;

        // Do not show the dialogue again if already completed.
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
        if (!other.CompareTag("Player"))
            return;

        if (hasTriggered)
            return;

        hasTriggered = true;

        // Save immediately when the player enters the trigger zone.
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.MarkTriggerCompleted(triggerID);

        if (textOB != null)
        {
            textOB.enabled = true;
            textOB.text = dialogue;
        }

        StartCoroutine(DisableText());
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);

        if (textOB != null)
            textOB.enabled = false;

        if (Activator != null)
            Destroy(Activator);

        if (triggerOnce)
            gameObject.SetActive(false);
    }
}
