using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume everything
        isPaused = false;
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Freeze everything (movement, camera)
        isPaused = true;
    }
}
