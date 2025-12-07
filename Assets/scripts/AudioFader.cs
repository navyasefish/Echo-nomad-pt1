using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioFader : MonoBehaviour
{
  [SerializeField] private AudioSource source;
  [SerializeField] private float fadeDuration = 0.6f;
  [SerializeField] private float targetVolume = 0.5f;

  private Coroutine fadeRoutine;

  private void Reset()
  {
    source = GetComponent<AudioSource>();
  }

  private void Awake()
  {
    if (source == null) source = GetComponent<AudioSource>();
  }

  public void FadeIn()
  {
    StartFade(targetVolume, fadeDuration);
  }

  public void FadeOut()
  {
    StartFade(0f, fadeDuration);
  }

  public void StopImmediate()
  {
    if (fadeRoutine != null) StopCoroutine(fadeRoutine);
    if (source != null)
    {
      source.volume = 0f;
      source.Stop();
    }
  }

  private void StartFade(float goalVolume, float duration)
  {
    if (source == null) return;
    if (fadeRoutine != null) StopCoroutine(fadeRoutine);
    fadeRoutine = StartCoroutine(FadeRoutine(goalVolume, duration));
  }

  private IEnumerator FadeRoutine(float goalVolume, float duration)
  {
    if (!source.isPlaying && goalVolume > 0f) source.Play();

    float start = source.volume;
    float t = 0f;
    while (t < duration)
    {
      t += Time.deltaTime;
      source.volume = Mathf.Lerp(start, goalVolume, t / duration);
      yield return null;
    }

    source.volume = goalVolume;
    if (Mathf.Approximately(goalVolume, 0f)) source.Stop();
    fadeRoutine = null;
  }
}
