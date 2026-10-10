using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public GameObject hud;
    public GameObject inv;
    public GameObject deathScreen;
    public GameObject player;

    public float Health = 100f;

    void Start()
    {
        deathScreen.SetActive(false);
    }

    void Update()
    {
        if (Health <= 0)
        {
            player.GetComponent<playerMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            hud.SetActive(false);
            inv.SetActive(false);
            deathScreen.SetActive(true);
        }

        if (Health > 100)
        {
            Health = 100;
        }
    }

}
