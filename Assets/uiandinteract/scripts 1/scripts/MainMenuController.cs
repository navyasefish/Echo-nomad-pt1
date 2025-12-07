using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.8f;
    public GameObject mainMenuUI;
    public AudioSource uiClickSound;
    public AudioSource bgMusic;

    void Start()
    {
        // In case fade image had some alpha
        if (fadeImage)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public void PlayGame()
    {
        // play click sound
        if (uiClickSound) uiClickSound.Play();

        // fade out bg music
        if (bgMusic) StartCoroutine(FadeOutMusic());

        mainMenuUI.SetActive(false);
        StartCoroutine(FadeAndLoadScene("SampleScene"));
    }

    public void QuitGame()
    {
        if (uiClickSound) uiClickSound.Play();

        if (bgMusic) StartCoroutine(FadeOutMusic());

        mainMenuUI.SetActive(false);
        StartCoroutine(FadeAndQuit());
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return FadeToBlack();
        SceneManager.LoadScene(sceneName);   // INSTANT LOAD — NO WAIT
    }

    IEnumerator FadeAndQuit()
    {
        yield return FadeToBlack();
        Application.Quit();   // Close app instantly
    }

    IEnumerator FadeToBlack()
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOutMusic()
    {
        float startVol = bgMusic.volume;
        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            bgMusic.volume = Mathf.Lerp(startVol, 0f, t / 0.5f);
            yield return null;
        }

        bgMusic.Stop();
        bgMusic.volume = startVol;
    }
}
