using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunnerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("References")]
    private RunnerAI runnerAI;
    private Rigidbody rb;
    private Collider col;

    public Animator anim;
    public AudioSource aux;

    private void Start()
    {
        currentHealth = maxHealth;

        // Get references
        runnerAI = GetComponent<RunnerAI>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Ensure Rigidbody is initially kinematic
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Runner took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Runner has died.");
        aux.enabled = false;
        col.excludeLayers = LayerMask.GetMask("Player"); // Exclude from interactions
        anim.SetBool("Dead", true);
        // Disable the AI script
        if (runnerAI != null)
        {
            runnerAI.enabled = false;
        }

        gameObject.GetComponent<NavMeshAgent>().enabled = false;

        // Enable physics to make the runner fall naturally
        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;   // Ensure gravity is applied
            rb.velocity = Vector3.zero; // Reset velocity to avoid any lingering movement

            // Apply a small backward force to simulate impact
            Vector3 backwardForce = -transform.forward * 2f; // Adjust the multiplier for desired force
            rb.AddForce(backwardForce, ForceMode.Impulse);
        }

        // Change the layer to prevent further interactions
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
    }

    private IEnumerator FreezeAfterFall()
    {
        // Wait for a short time to allow the runner to fall
        yield return new WaitForSeconds(2f);

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all movement and rotation
        }
    }
}

