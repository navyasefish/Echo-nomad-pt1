using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
  public Image iconImage;
  public AudioSource audioSource;

  public SoundDefinition currentSound;

  private MixerUIManager mixer;

  void Start()
  {
    mixer = FindAnyObjectByType<MixerUIManager>();
  }

  public void AssignSound(SoundDefinition sound)
  {
    // Stop existing sound
    if (currentSound != null)
    {
      audioSource.Stop();
      mixer.usedSoundIds.Remove(currentSound.soundID);
    }

    currentSound = sound;
    iconImage.sprite = sound.icon;
    iconImage.enabled = true;

    mixer.usedSoundIds.Add(sound.soundID);

    // Schedule play
    double startTime = LoopConductor.Instance.GetNextLoopStart();
    audioSource.clip = sound.clip;
    audioSource.loop = true;
    audioSource.PlayScheduled(startTime);

    mixer.CheckForSuccess();
  }

  public void ClearSlot()
  {
    if (currentSound != null)
    {
      audioSource.Stop();
      mixer.usedSoundIds.Remove(currentSound.soundID);
    }

    currentSound = null;
    iconImage.enabled = false;

    mixer.CheckForSuccess();
  }
}
