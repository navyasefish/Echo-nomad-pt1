using System.Collections;
using UnityEngine;
using TMPro; // or using UnityEngine.UI; if using standard Text

public class AreaPopup : MonoBehaviour
{
  [Header("UI References")]
  public GameObject popupParent;           // Single popup parent
  public TextMeshProUGUI areaNameText;     // Change to Text if not using TextMeshPro

  [Header("Popup Settings")]
  public float displayTime = 2f;
  public float fadeSpeed = 2f;

  [Header("Optional Sounds")]
  public AudioSource audioSource;
  public AudioClip popupSound;
  public AudioClip exitSound;

  private Coroutine currentRoutine;
  private CanvasGroup canvasGroup;

  private void Awake()
  {
    // Setup CanvasGroup on the popup parent
    if (popupParent != null)
    {
      canvasGroup = popupParent.GetComponent<CanvasGroup>();
      if (canvasGroup == null)
        canvasGroup = popupParent.AddComponent<CanvasGroup>();

      popupParent.SetActive(false);
    }
  }

  public void ShowAreaName(string areaName)
  {
    if (popupParent == null || areaNameText == null)
    {
      Debug.LogWarning("AreaPopup: Missing references!");
      return;
    }

    // Update the text content
    areaNameText.text = areaName;

    // Enable the popup
    popupParent.SetActive(true);

    // Stop any existing routine and start new one
    if (currentRoutine != null)
      StopCoroutine(currentRoutine);

    currentRoutine = StartCoroutine(ShowRoutine());
  }

  private IEnumerator ShowRoutine()
  {
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

    popupParent.SetActive(false);
  }
}