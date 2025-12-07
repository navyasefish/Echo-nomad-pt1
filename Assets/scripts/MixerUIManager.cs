using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MixerUIManager : MonoBehaviour
{
  public SlotController[] slots;         // 4 slots
  public SoundDefinition[] correctSet;   // drag the correct 4 sounds in inspector

  public GameObject successPopup;        // the UI popup to show on success

  [HideInInspector]
  public HashSet<string> usedSoundIds = new HashSet<string>();

  public void DropSoundInSlot(SoundDefinition sound, SlotController slot)
  {
    if (usedSoundIds.Contains(sound.soundID))
      return; // already used

    slot.AssignSound(sound);
  }

  public void CheckForSuccess()
  {
    // All slots must be filled
    if (slots.Any(s => s.currentSound == null))
      return;

    // Compare sets (order doesn't matter)
    var playerSet = slots.Select(s => s.currentSound.soundID)
                         .OrderBy(id => id)
                         .ToList();

    var correct = correctSet.Select(s => s.soundID)
                            .OrderBy(id => id)
                            .ToList();

    bool success = playerSet.SequenceEqual(correct);

    if (success)
      successPopup.SetActive(true);
  }
}
