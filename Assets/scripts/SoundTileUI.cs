using UnityEngine;
using UnityEngine.UI;

public class SoundTileUI : MonoBehaviour
{
  public SoundDefinition sound;   // drag your ScriptableObject here
  public Image iconImage;         // drag your UI Image here

  private MixerUIManager mixer;

  private void OnEnable()
  {
    mixer = FindAnyObjectByType<MixerUIManager>();

    if (sound != null && iconImage != null)
      iconImage.sprite = sound.icon;
  }


  public bool CanUse()
  {
    return !mixer.usedSoundIds.Contains(sound.soundID);
  }
}
