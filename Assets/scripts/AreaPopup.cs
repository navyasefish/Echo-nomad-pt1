using System.Collections;
using UnityEngine;
using TMPro;

public class AreaPopup : MonoBehaviour
{
  [Header("UI References")]
  public CanvasGroup canvasGroup;   // assign Rainforest's CanvasGroup
  public TextMeshProUGUI areaText;  // assign TextMeshPro inside Rainforest

  [Header("Popup Settings")]
  public float displayTime = 2f;
  public float fadeSpeed = 2f;

  [Header("Optional Sounds")]
  public AudioSource audioSource;
  public AudioClip popupSound;      // sound for fade in
  public AudioClip exitSound;       // sound for fade out

  private Coroutine currentRoutine;

  public void ShowAreaName(string name)
  {
    Debug.Log("ShowAreaName called: " + name);

    // Enable the Rainforest object
    canvasGroup.gameObject.SetActive(true);

    if (currentRoutine != null)
      StopCoroutine(currentRoutine);

    currentRoutine = StartCoroutine(ShowPopup(name));
  }

  private IEnumerator ShowPopup(string name)
  {
    areaText.text = name;

    // Fade in
    canvasGroup.alpha = 0;
    if (audioSource != null && popupSound != null)
      audioSource.PlayOneShot(popupSound);

    while (canvasGroup.alpha < 1)
    {
      canvasGroup.alpha += Time.deltaTime * fadeSpeed;
      yield return null;
    }

    yield return new WaitForSeconds(displayTime);

    // Play exit sound
    if (audioSource != null && exitSound != null)
      audioSource.PlayOneShot(exitSound);

    // Fade out
    while (canvasGroup.alpha > 0)
    {
      canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
      yield return null;
    }

    // Disable Rainforest again
    canvasGroup.gameObject.SetActive(false);
  }
}
