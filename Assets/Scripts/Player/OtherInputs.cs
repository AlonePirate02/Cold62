using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInputs : MonoBehaviour
{
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private GunScript gunSc;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject otherUI;
    [SerializeField] private GameObject deathMenu;
    public bool isPaused = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            TogglePause();
        }
    }

    private void Start()
    {
        // Bugfix so these are enabled when we start the game
        pm.enabled = true;
        gunSc.enabled = true;
        isPaused = false;
        deathMenu.SetActive(false); // Fix for future maybe idk
        TogglePause(); // Just to make sure
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            otherUI.SetActive(false);
            pm.enabled = false;
            gunSc.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            otherUI.SetActive(true);
            pm.enabled = true;
            gunSc.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }

    }

    public void OnPlayerDeath()
    {
        deathMenu.SetActive(true);
        otherUI.SetActive(false);
        pm.enabled = false;
        gunSc.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

}
