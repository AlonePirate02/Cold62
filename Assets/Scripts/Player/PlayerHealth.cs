using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Settings
    public float maxHealth = 100f;
    public float currentHealth;

    // References
    private OtherInputs otherInputs;
    public Image healthBarLeft;
    public Image healthBarRight;
    

    private void Start()
    {
        currentHealth = maxHealth;

        otherInputs = GetComponent<OtherInputs>(); // Attached to the player
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Healed. Current health: " + currentHealth);
        UpdateHealthBar();
    }

    public void TakeDamage(float amount) 
    {
        currentHealth -= amount;
        Debug.Log("Damaged. Current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthBar();
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        
        otherInputs.OnPlayerDeath(); // Show death menu and disable player controls
    }

    private void UpdateHealthBar()
    {
        healthBarLeft.fillAmount = currentHealth / maxHealth;
        healthBarRight.fillAmount = currentHealth / maxHealth;
    }
}
