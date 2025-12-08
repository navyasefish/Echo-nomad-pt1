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
  public Button challengeAudioButton; // Button to play the challenge audio

  [Header("Button Images")]
  public Sprite playIcon;  // Drag your play icon sprite here
  public Sprite pauseIcon; // Drag your pause icon sprite here

  [Header("Correct Combination (Optional)")]
  public SoundDefinition[] correctSet; // For checking if player got it right

  [Header("UI")]
  public GameObject successPopup;
  public GameObject mixerPanel; // The entire mixer UI panel to disable after victory

  [Header("Success Actions")]
  public GameObject blockerObject;  // The object blocking the path
  public GameObject hermitObject;   // The hermit character to hide

  [Header("Challenge Audio")]
  public AudioClip challengeAudioClip; // The audio clip to play (optional, if not using button's AudioSource)

  private bool isPlaying = false;
  private bool hasSucceeded = false; // Track if puzzle was solved
  private bool victoryShown = false; // Track if victory popup was already shown
  private bool isChallengeAudioPlaying = false; // Track challenge audio state
  private AudioSource challengeAudioSource; // Reference to challenge audio source

  void Start()
  {
    // Setup button listeners
    if (playButton != null)
      playButton.onClick.AddListener(PlayAllSounds);

    if (resetButton != null)
      resetButton.onClick.AddListener(ResetAllSlots);

    if (challengeAudioButton != null)
    {
      challengeAudioButton.onClick.AddListener(ToggleChallengeAudio);

      // Get or add AudioSource to challenge button
      challengeAudioSource = challengeAudioButton.GetComponent<AudioSource>();
      if (challengeAudioSource == null)
      {
        challengeAudioSource = challengeAudioButton.gameObject.AddComponent<AudioSource>();
        Debug.Log("Added AudioSource to challenge button");
      }

      // Setup audio source
      challengeAudioSource.playOnAwake = false;
      challengeAudioSource.loop = false;

      // Assign clip if provided
      if (challengeAudioClip != null)
        challengeAudioSource.clip = challengeAudioClip;

      Debug.Log($"Challenge audio button setup (has clip: {challengeAudioSource.clip != null})");
    }

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

    hasSucceeded = false;
    victoryShown = false;

    Debug.Log("✅ Simplified Mixer System initialized");
    Debug.Log($"   Slots: {slots.Length}");
    Debug.Log($"   Tiles: {soundTiles.Length}");
  }

  void Update()
  {
    // Check for left click to dismiss success popup
    if (hasSucceeded && successPopup != null && successPopup.activeSelf)
    {
      if (Input.GetMouseButtonDown(0))
      {
        DismissSuccessPopup();
      }
    }

    // Check if challenge audio finished playing
    if (isChallengeAudioPlaying && challengeAudioSource != null && !challengeAudioSource.isPlaying)
    {
      isChallengeAudioPlaying = false;
      UpdateChallengeButtonIcon();
    }

    // Disable mixer panel interaction while victory popup is visible
    if (mixerPanel != null && successPopup != null)
    {
      bool mixerShouldBeInteractable = !successPopup.activeSelf;
      CanvasGroup mixerCanvasGroup = mixerPanel.GetComponent<CanvasGroup>();

      if (mixerCanvasGroup == null)
        mixerCanvasGroup = mixerPanel.AddComponent<CanvasGroup>();

      mixerCanvasGroup.interactable = mixerShouldBeInteractable;
      mixerCanvasGroup.blocksRaycasts = mixerShouldBeInteractable;
    }
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

    // Stop challenge audio if playing
    if (isChallengeAudioPlaying)
    {
      Debug.Log("   Stopping challenge audio to play mixer sounds");
      StopChallengeAudio();
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

    // Change button icon to pause
    UpdatePlayButtonIcon();

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

    // Change button icon back to play
    UpdatePlayButtonIcon();
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

    hasSucceeded = false;

    Debug.Log("   ✅ All slots cleared");
  }

  void CheckForSuccess()
  {
    // If victory was already shown, don't check again
    if (victoryShown)
    {
      Debug.Log("Victory already achieved - skipping success check");
      return;
    }

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
      Debug.Log("🎉🎉🎉 SUCCESS! Showing victory popup (first time)");
      successPopup.SetActive(true);
      hasSucceeded = true;
      victoryShown = true; // Mark that victory was shown

      // Blocker and hermit will be removed when popup is dismissed
      Debug.Log("   ⏳ Blocker and hermit will disappear when you click to dismiss");
    }
  }

  void DismissSuccessPopup()
  {
    Debug.Log("👆 Left click detected - dismissing success popup");

    if (successPopup != null)
      successPopup.SetActive(false);

    // NOW remove blocker and hermit (after player has seen the victory screen)
    if (blockerObject != null)
    {
      Debug.Log("   ✅ Removing blocker object");
      blockerObject.SetActive(false);
    }

    if (hermitObject != null)
    {
      Debug.Log("   ✅ Hiding hermit");
      hermitObject.SetActive(false);
    }
  }

  void UpdatePlayButtonIcon()
  {
    if (playButton == null)
      return;

    // Try to get Image from button itself first
    Image buttonImage = playButton.GetComponent<Image>();

    // If not found, look for Image in children (for icon child setup)
    if (buttonImage == null)
    {
      buttonImage = playButton.GetComponentInChildren<Image>();
    }

    if (buttonImage == null)
    {
      Debug.LogWarning("Play button has no Image component");
      return;
    }

    // Switch sprite based on playing state
    if (isPlaying && pauseIcon != null)
    {
      buttonImage.sprite = pauseIcon;
      Debug.Log("   🔄 Button icon → PAUSE");
    }
    else if (!isPlaying && playIcon != null)
    {
      buttonImage.sprite = playIcon;
      Debug.Log("   🔄 Button icon → PLAY");
    }
  }

  public void ToggleChallengeAudio()
  {
    if (challengeAudioSource == null)
    {
      Debug.LogWarning("No challenge audio source!");
      return;
    }

    if (challengeAudioSource.clip == null)
    {
      Debug.LogWarning("No challenge audio clip assigned!");
      return;
    }

    if (isChallengeAudioPlaying)
    {
      // Stop the audio
      Debug.Log("⏹️ Stopping challenge audio");
      StopChallengeAudio();
    }
    else
    {
      // Stop mixer sounds if playing
      if (isPlaying)
      {
        Debug.Log("   Stopping mixer sounds to play challenge audio");
        StopAllSounds();
      }

      // Play the audio
      Debug.Log("▶️ Playing challenge audio");
      challengeAudioSource.Play();
      isChallengeAudioPlaying = true;
      UpdateChallengeButtonIcon();
    }
  }

  void StopChallengeAudio()
  {
    if (challengeAudioSource != null)
    {
      challengeAudioSource.Stop();
      isChallengeAudioPlaying = false;
      UpdateChallengeButtonIcon();
    }
  }

  void UpdateChallengeButtonIcon()
  {
    if (challengeAudioButton == null)
      return;

    // Try to get Image from button itself first
    Image buttonImage = challengeAudioButton.GetComponent<Image>();

    // If not found, look for Image in children (for icon child setup)
    if (buttonImage == null)
    {
      buttonImage = challengeAudioButton.GetComponentInChildren<Image>();
    }

    if (buttonImage == null)
    {
      Debug.LogWarning("Challenge button has no Image component");
      return;
    }

    // Switch sprite based on playing state
    if (isChallengeAudioPlaying && pauseIcon != null)
    {
      buttonImage.sprite = pauseIcon;
      Debug.Log("   🔄 Challenge button icon → PAUSE");
    }
    else if (!isChallengeAudioPlaying && playIcon != null)
    {
      buttonImage.sprite = playIcon;
      Debug.Log("   🔄 Challenge button icon → PLAY");
    }
  }
}