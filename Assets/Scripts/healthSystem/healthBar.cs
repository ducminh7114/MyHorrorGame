using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    private Image healthBarIM;
    private float CurrentHealth;
    private float MaxHealth = 100f;
    playerHealth player;

    void Start()
    {
        healthBarIM = GetComponent<Image>();
        player = FindObjectOfType<playerHealth>();
    }

    void Update()
    {
        CurrentHealth = player.Health;
        healthBarIM.fillAmount = CurrentHealth / MaxHealth;
    }
}
