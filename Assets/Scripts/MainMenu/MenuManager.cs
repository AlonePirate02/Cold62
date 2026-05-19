using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Needed Reference
    private OtherInputs otherInputs;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        otherInputs = FindObjectOfType<OtherInputs>();
        otherInputs.isPaused = false;
        otherInputs.TogglePause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
