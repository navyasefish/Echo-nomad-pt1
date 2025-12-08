using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
  [Header("References")]
  public Image iconImage;        // The icon display
  public AudioSource audioSource; // The audio player

  [Header("Runtime State")]
  public SoundDefinition currentSound; // What sound is in this slot

  void Start()
  {
    // Start with no icon visible
    if (iconImage != null)
      iconImage.enabled = false;

    // Configure audio source
    if (audioSource != null)
    {
      audioSource.playOnAwake = false;
      audioSource.loop = true;
      audioSource.Stop();
    }
  }
}