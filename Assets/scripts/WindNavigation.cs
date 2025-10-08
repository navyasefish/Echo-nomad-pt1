using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindNavigation : MonoBehaviour
{
  public WindZone Windzone;
  public Transform objective;
  public Transform player;
  public bool is_guiding = false;

  Vector3 direction = Vector3.zero;
  public float windMax = 5f;       // Maximum wind strength
  public float windSpeed = 1f;     // How fast wind increases/decreases
  private float currentWind = 0f;

  [Header("UI")]
  public Image windSymbol;           // 🔹 Assign your image element
  public float fadeDuration = 0.5f;  // 🔹 Duration of fade in/out

  private CanvasGroup symbolGroup;   // 🔹 For fading

  [Header("Audio")]
  public AudioSource audioSource;
  public AudioClip startSound;
  public AudioClip stopSound;

  void Start()
  {
    // Setup CanvasGroup for fade
    if (windSymbol != null)
    {
      symbolGroup = windSymbol.GetComponent<CanvasGroup>();
      if (symbolGroup == null)
        symbolGroup = windSymbol.gameObject.AddComponent<CanvasGroup>();

      symbolGroup.alpha = 0f; // start hidden
      windSymbol.enabled = true; // make sure Image stays active for fading
    }
  }

  void Update()
  {
    // Toggle guiding on Z key press
    if (Input.GetKeyUp(KeyCode.Z))
    {
      is_guiding = !is_guiding;
      Debug.Log(is_guiding ? "✅ Guiding wind started." : "🛑 Guiding wind stopped.");

      // 🔹 Fade UI symbol
      if (symbolGroup != null)
        StartCoroutine(FadeSymbol(is_guiding));

      // 🔹 Play start/stop sound
      if (audioSource != null)
      {
        if (is_guiding && startSound != null)
          audioSource.PlayOneShot(startSound);
        else if (!is_guiding && stopSound != null)
          audioSource.PlayOneShot(stopSound);
      }
    }

    // Update wind direction and strength
    if (is_guiding)
    {
      direction = objective.position - player.position;
      direction.y = 0f;
      Windzone.transform.rotation = Quaternion.LookRotation(direction);

      currentWind = Mathf.Min(currentWind + windSpeed * Time.deltaTime, windMax);
    }
    else
    {
      currentWind = Mathf.Max(currentWind - windSpeed * Time.deltaTime, 0f);
    }

    Windzone.windMain = currentWind;
    Debug.DrawLine(player.position, objective.position, Color.cyan);
  }

  IEnumerator FadeSymbol(bool fadeIn)
  {
    float start = symbolGroup.alpha;
    float end = fadeIn ? 1f : 0f;
    float t = 0f;

    while (t < fadeDuration)
    {
      t += Time.deltaTime;
      symbolGroup.alpha = Mathf.Lerp(start, end, t / fadeDuration);
      yield return null;
    }

    symbolGroup.alpha = end;
  }
}
