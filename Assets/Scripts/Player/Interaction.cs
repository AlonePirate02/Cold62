using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    private GunScript gunScript;
    private PlayerHealth playerHealth;
    private Camera cam;
    public TextMeshProUGUI interactText;

    public bool hasKeycard = false;

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
                    StartCoroutine(RemoveText("AMMO PICKED UP"));
                }
                else
                {
                    Debug.Log("Max ammo reached");
                    StartCoroutine(RemoveText("MAX AMMO REACHED"));
                }
            }
            else if (hit.collider.CompareTag("Bandage"))
            {
                playerHealth.Heal(25);
                Destroy(hit.collider.gameObject);
                StartCoroutine(RemoveText("HEALTH RESTORED"));
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
            else if (hit.collider.CompareTag("Keycard"))
            {
                hasKeycard = true;
                StartCoroutine(RemoveText("YOU GOT THE BLUE KEYCARD"));
                Destroy(hit.collider.gameObject);
            }
            else if(hit.collider.CompareTag("End"))
            {
                if (hasKeycard)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1 );
                }
                else
                {
                    StartCoroutine(RemoveText("YOU NEED THE BLUE KEYCARD TO ENTER THE SAFE ROOM"));
                }
            }
        }
    }

    IEnumerator RemoveText(string text)
    {
        interactText.text = text;

        yield return new WaitForSeconds(1f);

        interactText.text = "";
    }
}
