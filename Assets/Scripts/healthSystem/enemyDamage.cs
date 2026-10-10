using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyDamage : MonoBehaviour
{
    public GameObject player;
    private float damageRange;
    public float damageSet = 25f;
    public float minDamage;
    public float maxDamage;

    public bool randomDamage;
    public bool setDamage;

    public AudioClip[] sounds;
    private AudioSource source;

    void Start()
    {
        damageRange = Random.Range(minDamage, maxDamage);
        source = player.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && randomDamage)
        {
           player.GetComponent<playerHealth>().Health -= damageRange;
           source.clip = sounds[Random.Range(0, sounds.Length)];
           source.Play();
        }

        if (other.gameObject.tag == "Player" && setDamage)
        {
            player.GetComponent<playerHealth>().Health -= damageSet;
            source.clip = sounds[Random.Range(0, sounds.Length)];
            source.Play();
        }
    }
}
