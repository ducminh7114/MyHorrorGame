using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class lightFicker : MonoBehaviour
{
    [Header("Flicker setting")]
    public bool enableFlicker = true;
    //public AudioSource lightSound;
    public Light lightObject;
    public Renderer lightBulb;

    public float minTime;
    public float maxTime;
    public float timer;
    public Material offLight;
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableFlicker)
        {
            lightFlickering();
        }

    }

    void lightFlickering()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            lightObject.enabled = !lightObject.enabled;
            lightBulb.material = offLight;
            timer = Random.Range(minTime, maxTime);
            //lightSound.Play();
        }
    }
}
