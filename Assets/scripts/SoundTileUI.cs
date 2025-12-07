using UnityEngine;
using UnityEngine.UI;

public class SoundTileUI : MonoBehaviour
{
  public SoundDefinition sound;
  public Image iconImage;

  private MixerUIManager mixer;

  void Start()
  {
    mixer = FindAnyObjectByType<MixerUIManager>();
    iconImage.sprite = sound.icon;
  }

  public bool CanUse()
  {
    return !mixer.usedSoundIds.Contains(sound.soundID);
  }
}
