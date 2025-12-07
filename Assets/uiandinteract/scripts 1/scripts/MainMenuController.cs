using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.8f;
    public GameObject mainMenuUI;

    // PLAY BUTTON
    public void PlayGame()
    {
        mainMenuUI.SetActive(false);
        StartCoroutine(FadeAndLoad("SampleScene"));
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }

    // FADE + ASYNC LOAD
    IEnumerator FadeAndLoad(string sceneName)
    {
        // Load in background
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float t = 0;
        Color c = fadeImage.color;

        // Fade to black
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // Switch scene immediately after full fade
        op.allowSceneActivation = true;
    }
}
