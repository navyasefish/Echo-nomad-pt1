using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SimplifiedMixerSystem : MonoBehaviour
{
  [Header("Slots")]
  public SlotController[] slots; // 4 slots

  [Header("Sound Tiles")]
  public SoundTileUI[] soundTiles; // 8 tiles

  [Header("Buttons")]
  public Button playButton;
  public Button resetButton;

  [Header("Correct Combination (Optional)")]
  public SoundDefinition[] correctSet; // For checking if player got it right

  [Header("UI")]
  public GameObject successPopup;

  private bool isPlaying = false;

  void Start()
  {
    // Setup button listeners
    if (playButton != null)
      playButton.onClick.AddListener(PlayAllSounds);

    if (resetButton != null)
      resetButton.onClick.AddListener(ResetAllSlots);

    // Setup click listeners for all tiles
    foreach (var tile in soundTiles)
    {
      Button btn = tile.GetComponent<Button>();
      if (btn == null)
      {
        btn = tile.gameObject.AddComponent<Button>();
        Debug.Log($"Added Button to {tile.gameObject.name}");
      }

      // Capture tile reference for the lambda
      SoundTileUI capturedTile = tile;
      btn.onClick.AddListener(() => OnTileClicked(capturedTile));
    }

    if (successPopup != null)
      successPopup.SetActive(false);

    Debug.Log("✅ Simplified Mixer System initialized");
    Debug.Log($"   Slots: {slots.Length}");
    Debug.Log($"   Tiles: {soundTiles.Length}");
  }

  void OnTileClicked(SoundTileUI tile)
  {
    Debug.Log($"🎵 Tile clicked: {tile.sound.soundID}");

    // Check if this sound is already in a slot
    foreach (var slot in slots)
    {
      if (slot.currentSound != null && slot.currentSound.soundID == tile.sound.soundID)
      {
        Debug.Log($"   ⚠️ {tile.sound.soundID} already in a slot");
        return;
      }
    }

    // Find first empty slot
    SlotController emptySlot = null;
    foreach (var slot in slots)
    {
      if (slot.currentSound == null)
      {
        emptySlot = slot;
        break;
      }
    }

    if (emptySlot == null)
    {
      Debug.Log("   ⚠️ All slots are full! Reset to add more.");
      return;
    }

    // Assign sound to slot (but don't play yet)
    AssignSoundToSlot(tile.sound, emptySlot);
    Debug.Log($"   ✅ Assigned to {emptySlot.gameObject.name}");

    // Check if all slots are filled
    CheckIfAllFilled();
  }

  void AssignSoundToSlot(SoundDefinition sound, SlotController slot)
  {
    slot.currentSound = sound;

    // Show icon
    if (slot.iconImage != null)
    {
      slot.iconImage.sprite = sound.icon;
      slot.iconImage.enabled = true;
    }

    // Setup audio but don't play
    if (slot.audioSource != null)
    {
      slot.audioSource.clip = sound.clip;
      slot.audioSource.loop = true;
      slot.audioSource.Stop(); // Make sure it's not playing
    }
  }

  void CheckIfAllFilled()
  {
    bool allFilled = true;
    foreach (var slot in slots)
    {
      if (slot.currentSound == null)
      {
        allFilled = false;
        break;
      }
    }

    if (allFilled)
    {
      Debug.Log("🎉 All slots filled! Press PLAY to hear them.");
    }
  }

  public void PlayAllSounds()
  {
    Debug.Log("▶️ PLAY button pressed");

    if (isPlaying)
    {
      Debug.Log("   Already playing, stopping...");
      StopAllSounds();
      return;
    }

    // Check if all slots are filled
    int filledCount = 0;
    foreach (var slot in slots)
    {
      if (slot.currentSound != null)
        filledCount++;
    }

    if (filledCount == 0)
    {
      Debug.Log("   ⚠️ No sounds to play. Click tiles to fill slots first.");
      return;
    }

    // Play all sounds simultaneously
    double startTime = AudioSettings.dspTime + 0.1; // Small delay for sync

    foreach (var slot in slots)
    {
      if (slot.currentSound != null && slot.audioSource != null)
      {
        slot.audioSource.PlayScheduled(startTime);
        Debug.Log($"   🎵 Playing {slot.currentSound.soundID}");
      }
    }

    isPlaying = true;

    // Change button text
    if (playButton != null)
    {
      Text btnText = playButton.GetComponentInChildren<Text>();
      if (btnText != null)
        btnText.text = "STOP";
    }

    // Check for success if all slots filled
    if (filledCount == slots.Length)
    {
      CheckForSuccess();
    }
  }

  public void StopAllSounds()
  {
    Debug.Log("⏹️ Stopping all sounds");

    foreach (var slot in slots)
    {
      if (slot.audioSource != null)
        slot.audioSource.Stop();
    }

    isPlaying = false;

    // Change button text back
    if (playButton != null)
    {
      Text btnText = playButton.GetComponentInChildren<Text>();
      if (btnText != null)
        btnText.text = "PLAY";
    }
  }

  public void ResetAllSlots()
  {
    Debug.Log("🔄 RESET button pressed");

    // Stop all sounds first
    StopAllSounds();

    // Clear all slots
    foreach (var slot in slots)
    {
      slot.currentSound = null;

      if (slot.iconImage != null)
        slot.iconImage.enabled = false;

      if (slot.audioSource != null)
      {
        slot.audioSource.Stop();
        slot.audioSource.clip = null;
      }
    }

    // Hide success popup
    if (successPopup != null)
      successPopup.SetActive(false);

    Debug.Log("   ✅ All slots cleared");
  }

  void CheckForSuccess()
  {
    if (correctSet == null || correctSet.Length != slots.Length)
    {
      Debug.Log("No correct set defined or wrong length");
      return;
    }

    // Get current sounds
    var currentSounds = slots
      .Where(s => s.currentSound != null)
      .Select(s => s.currentSound.soundID)
      .OrderBy(id => id)
      .ToList();

    // Get correct sounds
    var correctSounds = correctSet
      .Select(s => s.soundID)
      .OrderBy(id => id)
      .ToList();

    bool success = currentSounds.SequenceEqual(correctSounds);

    Debug.Log($"Current: {string.Join(", ", currentSounds)}");
    Debug.Log($"Correct: {string.Join(", ", correctSounds)}");
    Debug.Log($"Success: {success}");

    if (success && successPopup != null)
    {
      Debug.Log("🎉🎉🎉 SUCCESS! Showing popup");
      successPopup.SetActive(true);
    }
  }
}