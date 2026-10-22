using UnityEngine;

public class lightSwitch : MonoBehaviour
{
    [Header("UI")]
    public GameObject intText;
    public TMP_Text ObjName;

    [Header("Light")]
    public GameObject[] lightObject;
    public Renderer[] lightBulbs;

    [Header("Material")]
    public Material offLight;
    public Material onLight;

    [Header("Audio & Animation")]
    public AudioSource lightSwitchSound;
    public Animator switchAnim;

    [Header("Object information")]
    public string objectName = "Item Name";
    public bool toggle;
    private bool interactable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Camera detected");
            intText.SetActive(true);

            if (ObjName != null)
            {
                ObjName.text = objectName;
                ObjName.enabled = true;
            }

            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);

            if (ObjName != null)
            {
                ObjName.text = "";
                ObjName.enabled = false;
            }

            interactable = false;
        }
    }

    private void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            toggle = !toggle;

            if (lightSwitchSound) lightSwitchSound.Play();
            if (switchAnim)
            {
                switchAnim.ResetTrigger("press");
                switchAnim.SetTrigger("press");
            }
            for (int i = 0; i < lightObject.Length; i++)
            {
                if (toggle)
                {
                    lightObject[i].SetActive(true);
                    lightBulbs[i].material = onLight;
                }
                else
                {
                    lightObject[i].SetActive(false);
                    lightBulbs[i].material = offLight;
                }
            }

            if (ObjName != null)
                {
                ObjName.text = "";
                }
        }
    }
}
