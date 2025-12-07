using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main controller for the mixing panel (coordinates all components)
/// </summary>
public class MixingPanelController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject mixingPanelRoot;

    [Header("Components")]
    [SerializeField] private AudioPreviewPlayer previewPlayer;
    [SerializeField] private MixingSequencer sequencer;
    [SerializeField] private MixingSoundLibrary soundLibrary;

    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button clearAllButton;

    [Header("Audio")]
    [SerializeField] private AudioClip openPanelSound;
    [SerializeField] private AudioClip closePanelSound;

    void Awake()
    {
        // Setup buttons
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseMixingPanel);
        }

        if (clearAllButton != null)
        {
            clearAllButton.onClick.AddListener(ClearAllSlots);
        }

        // Start hidden
        if (mixingPanelRoot != null)
        {
            mixingPanelRoot.SetActive(false);
        }
    }

    /// <summary>
    /// Open the mixing panel (called from pause menu)
    /// </summary>
    public void OpenMixingPanel()
    {
        if (mixingPanelRoot != null)
        {
            mixingPanelRoot.SetActive(true);
        }

        // Play open sound
        if (openPanelSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(openPanelSound, 0.5f);
        }

        Debug.Log("Mixing Panel Opened");
    }

    /// <summary>
    /// Close the mixing panel (return to pause menu)
    /// </summary>
    public void CloseMixingPanel()
    {
        // Stop any playing sequences
        if (sequencer != null)
        {
            sequencer.StopAll();
        }

        // Stop preview player
        if (previewPlayer != null)
        {
            previewPlayer.Stop();
        }

        if (mixingPanelRoot != null)
        {
            mixingPanelRoot.SetActive(false);
        }

        // Play close sound
        if (closePanelSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(closePanelSound, 0.5f);
        }

        Debug.Log("Mixing Panel Closed");
    }

    /// <summary>
    /// Preview a sound in the top audio player
    /// </summary>
    public void PreviewSound(AudioClip clip)
    {
        if (previewPlayer != null)
        {
            previewPlayer.LoadClip(clip);
            previewPlayer.Play();
        }
    }

    /// <summary>
    /// Clear all mixing slots
    /// </summary>
    public void ClearAllSlots()
    {
        if (sequencer != null)
        {
            sequencer.ClearAllSlots();
        }

        Debug.Log("All mixing slots cleared");
    }

    /// <summary>
    /// Get filled slot count (for UI display)
    /// </summary>
    public int GetFilledSlotCount()
    {
        if (sequencer != null)
        {
            return sequencer.GetFilledSlotCount();
        }
        return 0;
    }

    /// <summary>
    /// Get total unlocked sounds (for UI display)
    /// </summary>
    public int GetUnlockedSoundCount()
    {
        if (soundLibrary != null)
        {
            return soundLibrary.GetUnlockedCount();
        }
        return 0;
    }
}