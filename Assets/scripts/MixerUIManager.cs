using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MixerUIManager : MonoBehaviour
{
  [Header("Slots")]
  public SlotController[] slots; // Drag all 4 slots here

  [Header("Correct Combination")]
  public SoundDefinition[] correctSet; // Drag the correct 4 sounds in inspector

  [Header("UI References")]
  public GameObject successPopup; // The UI popup to show on success

  [Header("Runtime State")]
  [HideInInspector]
  public HashSet<string> usedSoundIds = new HashSet<string>();

  private void Start()
  {
    if (successPopup != null)
      successPopup.SetActive(false);

    Debug.Log($"MixerUIManager initialized with {slots.Length} slots");

    if (correctSet != null && correctSet.Length > 0)
    {
      var correctIDs = correctSet.Select(s => s.soundID).ToList();
      Debug.Log($"Correct combination: {string.Join(", ", correctIDs)}");
    }
    else
    {
      Debug.LogWarning("No correct sound set defined!");
    }
  }

  public void DropSoundInSlot(SoundDefinition sound, SlotController slot)
  {
    if (sound == null || slot == null)
    {
      Debug.LogError("DropSoundInSlot called with null parameters!");
      return;
    }

    // CRITICAL FIX: Check happens INSIDE SlotController.AssignSound
    // The slot will handle removing old sound from usedSoundIds
    // So we DON'T check here, we let the slot handle it

    Debug.Log($"Dropping {sound.soundID} into slot {slot.gameObject.name}");
    Debug.Log($"Currently used IDs: {string.Join(", ", usedSoundIds)}");

    // Check if this sound is already used in a DIFFERENT slot
    if (usedSoundIds.Contains(sound.soundID))
    {
      Debug.Log($"Sound {sound.soundID} already in use, cannot drop");
      return;
    }

    slot.AssignSound(sound);
  }

  public void CheckForSuccess()
  {
    // Count filled slots
    int filledSlots = slots.Count(s => s.currentSound != null);

    Debug.Log($"CheckForSuccess: {filledSlots}/{slots.Length} slots filled");

    // All slots must be filled
    if (slots.Any(s => s.currentSound == null))
    {
      Debug.Log("Not all slots filled yet");
      return;
    }

    // Compare sets (order doesn't matter)
    var playerSet = slots.Select(s => s.currentSound.soundID)
                         .OrderBy(id => id)
                         .ToList();

    var correct = correctSet.Select(s => s.soundID)
                            .OrderBy(id => id)
                            .ToList();

    bool success = playerSet.SequenceEqual(correct);

    Debug.Log($"Player set: {string.Join(", ", playerSet)}");
    Debug.Log($"Correct set: {string.Join(", ", correct)}");
    Debug.Log($"Match: {success}");

    if (success && successPopup != null)
    {
      Debug.Log("SUCCESS! Showing popup");
      successPopup.SetActive(true);
    }
    else if (success && successPopup == null)
    {
      Debug.LogWarning("Success achieved but no popup assigned!");
    }
  }

  public void ResetMixer()
  {
    Debug.Log("Resetting mixer...");

    foreach (var slot in slots)
    {
      slot.ClearSlot();
    }

    usedSoundIds.Clear();

    if (successPopup != null)
      successPopup.SetActive(false);
  }

  // Optional: Add button to clear a specific slot
  public void ClearSlot(SlotController slot)
  {
    if (slot != null)
      slot.ClearSlot();
  }
}