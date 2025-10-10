using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Called by Play Button
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Called by Quit Button
    public void QuitGame()
    {
        Debug.Log("Quit button pressed");
        Application.Quit();
    }
}
