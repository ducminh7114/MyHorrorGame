using UnityEngine;

public class TVSimple : MonoBehaviour
{
    public GameObject screen;
    public Material onMat;
    public Material offMat;

    bool isOn = false;

    void Start()
    {
        screen.GetComponent<Renderer>().material = offMat;
    }

    public void ToggleTV()
    {
        isOn = !isOn;
        screen.GetComponent<Renderer>().material = isOn ? onMat : offMat;
    }
}

