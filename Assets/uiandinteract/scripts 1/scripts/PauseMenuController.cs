using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject itemMenuPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pausePanelSound;
    public AudioClip itemPanelSound;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        itemMenuPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        PlaySound(pausePanelSound);
    }

    public void OpenItemMenu()
    {
        pausePanel.SetActive(false);
        itemMenuPanel.SetActive(true);

        PlaySound(itemPanelSound);
    }

    public void BackToPauseMenu()
    {
        itemMenuPanel.SetActive(false);
        pausePanel.SetActive(true);

        PlaySound(itemPanelSound);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}
