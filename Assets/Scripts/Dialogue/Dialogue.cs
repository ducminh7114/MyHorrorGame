using UnityEngine;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TMP_Text textOB;
    public GameObject Activator;
    public string dialogue = "Dialogue";

    public float timer = 2f;

    void Start()
    {
        textOB.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textOB.enabled = true;
            textOB.text = dialogue;
            StartCoroutine(DisableText());
        }
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);

        textOB.enabled = false;

        if (Activator != null)
        {
            Destroy(Activator);
            Activator.SetActive(false);
        }
    }
}