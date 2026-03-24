using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private GunScript gunScript;
    private PlayerHealth playerHealth;

    private float interactionRange = 3f; // The maximum distance for interaction

    private void Start()
    {
        gunScript = FindAnyObjectByType<GunScript>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 1f); // Visualize the ray in the editor

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.CompareTag("Magazine"))
            {
                if(gunScript.ammoInPocket < gunScript.maxAmmo - gunScript.magCapacity) // Check if adding ammo would exceed the maximum ammo limit
                {
                    gunScript.AddAmmo(gunScript.magCapacity); // Add ammo to the player's pocket based on the magazine capacity
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("Max ammo reached");
                }
            }
            else if (hit.collider.CompareTag("Bandage"))
            {
                playerHealth.Heal(20);
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
