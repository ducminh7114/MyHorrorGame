using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("UI")] 
    public GameObject inttext; 
    public GameObject lockedText;

    [Header("Key")]
    public GameObject key;

    [Header("Door")]
    public GameObject door;
    public bool interactable = false;
    public bool toggle = false;
    public Animator doorAnim;

    private Coroutine lockedCoroutine;

    private void Start()
    {
        if (inttext != null)
            inttext.SetActive(false);

        if (lockedText != null)
            lockedText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if(!interactable)
            {
                interactable = true;

            }

            if (inttext != null)
                inttext.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (interactable)
            {
                interactable = false;
            }
            if (inttext != null)
                inttext.SetActive(false);
        }
    }

    private void Update()
    {
        if (!interactable)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Door E pressed");

                if (key == null || !key.activeSelf)
            {
                ToggleDoor();

            }

            else
            {
                ShowLockedText();
            }
        }
    }

    private void ToggleDoor()
    {
        toggle = !toggle;

        if (toggle)
        {
            Debug.Log("Opening Door");

            doorAnim.ResetTrigger("close");
            doorAnim.SetTrigger("open");

        }
        else
        {
            Debug.Log("Closing Door");

            doorAnim.ResetTrigger("open");
            doorAnim.SetTrigger("close");
        }

        if (inttext != null)
            inttext.SetActive(false);

        interactable = false;
    }

    private void ShowLockedText()
    {
        Debug.Log("Door is Locked");

        if (lockedText == null)
            return;

        lockedText.SetActive(true);

        if (lockedCoroutine != null)
            StopCoroutine(lockedCoroutine);

        lockedCoroutine = StartCoroutine(DisableText());

    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(2f);

        if (lockedText != null) ;
        lockedText.SetActive(false);

        lockedCoroutine = null;
    }
}