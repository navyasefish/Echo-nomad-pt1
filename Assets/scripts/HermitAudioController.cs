using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HermitAudioController : MonoBehaviour
{
  [Tooltip("Optional AudioFader to use for fade in/out.")]
  public AudioFader audioFader;

  [Tooltip("AudioSource with the humming clip.")]
  public AudioSource hummingSource;

  [Tooltip("Target ambient volume (used if no fader).")]
  public float defaultVolume = 0.5f;

  private void Reset()
  {
    hummingSource = GetComponent<AudioSource>();
  }

  private void Awake()
  {
    if (hummingSource == null) hummingSource = GetComponent<AudioSource>();
  }

  public void FadeInHumming()
  {
    if (audioFader != null) audioFader.FadeIn();
    else
    {
      if (hummingSource != null)
      {
        hummingSource.volume = defaultVolume;
        if (!hummingSource.isPlaying) hummingSource.Play();
      }
    }
  }

  public void FadeOutHumming()
  {
    if (audioFader != null) audioFader.FadeOut();
    else
    {
      if (hummingSource != null)
        hummingSource.Stop();
    }
  }

  public void StopImmediate()
  {
    if (audioFader != null) audioFader.StopImmediate();
    if (hummingSource != null) hummingSource.Stop();
  }
}
