using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Controls synchronized playback of all mixing slots (Incredibox-style)
/// </summary>
public class MixingSequencer : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private List<MixingSlot> mixingSlots = new List<MixingSlot>();

    [Header("Playback Control")]
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Image playPauseIcon;

    [Header("Icons")]
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    [Header("Win Condition")]
    [SerializeField] private List<string> correctPattern = new List<string>(); // List of collectible IDs in order
    [SerializeField] private GameObject winPopup;

    private bool isPlaying = false;
    private float syncTime = 0f;

    void Awake()
    {
        // Setup slot references
        foreach (var slot in mixingSlots)
        {
            slot.SetSequencer(this);
        }

        if (playPauseButton != null)
        {
            playPauseButton.onClick.AddListener(TogglePlayPause);
        }

        UpdatePlayPauseIcon();

        if (winPopup != null)
        {
            winPopup.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            // Update sync time (this helps keep all loops aligned)
            syncTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Toggle play/pause for entire sequence
    /// </summary>
    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            PauseAll();
        }
        else
        {
            PlayAll();
        }
    }

    /// <summary>
    /// Play all filled slots
    /// </summary>
    public void PlayAll()
    {
        isPlaying = true;

        // Start all slots that have sounds
        foreach (var slot in mixingSlots)
        {
            if (slot.HasSound())
            {
                slot.Play();
            }
        }

        UpdatePlayPauseIcon();
    }

    /// <summary>
    /// Pause all slots
    /// </summary>
    public void PauseAll()
    {
        isPlaying = false;

        foreach (var slot in mixingSlots)
        {
            slot.Pause();
        }

        UpdatePlayPauseIcon();
    }

    /// <summary>
    /// Stop all slots and reset
    /// </summary>
    public void StopAll()
    {
        isPlaying = false;
        syncTime = 0f;

        foreach (var slot in mixingSlots)
        {
            slot.Stop();
        }

        UpdatePlayPauseIcon();
    }

    /// <summary>
    /// Called when any slot changes (sound added/removed)
    /// </summary>
    public void OnSlotChanged()
    {
        // Check if pattern matches win condition
        CheckWinCondition();

        // If playing, sync new sounds
        if (isPlaying)
        {
            foreach (var slot in mixingSlots)
            {
                if (slot.HasSound())
                {
                    slot.SyncToTime(syncTime);
                }
            }
        }
    }

    /// <summary>
    /// Check if current pattern matches the correct rhythm
    /// </summary>
    void CheckWinCondition()
    {
        if (correctPattern == null || correctPattern.Count == 0)
            return;

        if (correctPattern.Count != mixingSlots.Count)
        {
            Debug.LogWarning("MixingSequencer: Correct pattern size doesn't match slot count!");
            return;
        }

        // Build current pattern
        List<string> currentPattern = new List<string>();
        foreach (var slot in mixingSlots)
        {
            if (slot.HasSound())
            {
                string id = slot.GetCurrentSound().GetData().collectibleID;
                currentPattern.Add(id);
            }
            else
            {
                currentPattern.Add(""); // Empty slot
            }
        }

        // Compare patterns
        bool isMatch = true;
        for (int i = 0; i < correctPattern.Count; i++)
        {
            if (correctPattern[i] != currentPattern[i])
            {
                isMatch = false;
                break;
            }
        }

        // Show win popup if matched
        if (isMatch)
        {
            ShowWinPopup();
        }
    }

    /// <summary>
    /// Show victory popup
    /// </summary>
    void ShowWinPopup()
    {
        Debug.Log("🎉 You have guessed the final rhythm!");

        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }

        // Optionally stop playback
        // PauseAll();
    }

    /// <summary>
    /// Update play/pause button icon
    /// </summary>
    void UpdatePlayPauseIcon()
    {
        if (playPauseIcon != null)
        {
            playPauseIcon.sprite = isPlaying ? pauseIcon : playIcon;
        }
    }

    /// <summary>
    /// Clear all slots
    /// </summary>
    public void ClearAllSlots()
    {
        StopAll();

        foreach (var slot in mixingSlots)
        {
            if (slot.HasSound())
            {
                DraggableSound sound = slot.GetCurrentSound();
                slot.RemoveSound();
                sound.ReturnToOriginalPosition();
            }
        }
    }

    /// <summary>
    /// Get number of filled slots
    /// </summary>
    public int GetFilledSlotCount()
    {
        return mixingSlots.Count(slot => slot.HasSound());
    }
}