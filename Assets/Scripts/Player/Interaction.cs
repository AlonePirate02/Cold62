using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private GunScript gunScript;
    private PlayerHealth playerHealth;
    private Camera cam;

    private float interactionRange = 3f; // The maximum distance for interaction

    private void Start()
    {
        gunScript = FindAnyObjectByType<GunScript>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        cam = Camera.main;
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

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactionRange))
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
            else if (hit.collider.CompareTag("Door"))
            {
                hit.collider.GetComponent<DoorOpen>().ToggleDoor();
            }
            else if (hit.collider.CompareTag("Drawer"))
            {
                hit.collider.GetComponent<DrawerOpen>().ToggleDrawer();
            }
            else if (hit.collider.CompareTag("Closet"))
            {
                hit.collider.GetComponent<DoorOpen>().ToggleDoor();
            }
        }
    }
}
