using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual slot in the mixing sequencer that can hold one sound
/// </summary>
public class MixingSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Transform soundContainer; // Where the draggable sound sits

    [Header("Visual States")]
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color filledColor = new Color(0.3f, 0.5f, 0.3f, 1f);
    [SerializeField] private Color highlightColor = new Color(0.4f, 0.6f, 0.4f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource loopSource;

    private DraggableSound currentSound;
    private MixingSequencer sequencer;

    void Awake()
    {
        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
        }

        loopSource.loop = true;
        loopSource.playOnAwake = false;
        loopSource.volume = 1f;

        UpdateVisuals();
    }

    /// <summary>
    /// Set reference to parent sequencer
    /// </summary>
    public void SetSequencer(MixingSequencer seq)
    {
        sequencer = seq;
    }

    /// <summary>
    /// Check if this slot can accept a sound
    /// </summary>
    public bool CanAcceptSound()
    {
        return currentSound == null;
    }

    /// <summary>
    /// Check if this slot has a sound
    /// </summary>
    public bool HasSound()
    {
        return currentSound != null;
    }

    /// <summary>
    /// Get current sound
    /// </summary>
    public DraggableSound GetCurrentSound()
    {
        return currentSound;
    }

    /// <summary>
    /// Place a sound in this slot
    /// </summary>
    public void PlaceSound(DraggableSound sound)
    {
        if (sound == null) return;

        // Remove from previous slot if any
        MixingSlot previousSlot = sound.GetCurrentSlot();
        if (previousSlot != null && previousSlot != this)
        {
            previousSlot.RemoveSound();
        }

        // If this slot already has a sound, swap them
        if (currentSound != null)
        {
            DraggableSound oldSound = currentSound;
            RemoveSound();

            // Send old sound back to library
            oldSound.ReturnToOriginalPosition();
        }

        // Place new sound
        currentSound = sound;
        currentSound.PlaceInParent(soundContainer);
        currentSound.SetCurrentSlot(this);

        // Setup audio
        AudioClip clip = currentSound.GetAudioClip();
        if (clip != null)
        {
            loopSource.clip = clip;
        }

        UpdateVisuals();

        // Notify sequencer
        if (sequencer != null)
        {
            sequencer.OnSlotChanged();
        }
    }

    /// <summary>
    /// Remove sound from this slot
    /// </summary>
    public void RemoveSound()
    {
        if (currentSound != null)
        {
            currentSound.SetCurrentSlot(null);
            currentSound = null;
            loopSource.clip = null;
            loopSource.Stop();
        }

        UpdateVisuals();

        // Notify sequencer
        if (sequencer != null)
        {
            sequencer.OnSlotChanged();
        }
    }

    /// <summary>
    /// Called when sound is being dragged out
    /// </summary>
    public void OnSoundRemovedByDrag(DraggableSound sound)
    {
        if (currentSound == sound)
        {
            currentSound = null;
            loopSource.clip = null;
            loopSource.Stop();
            UpdateVisuals();

            if (sequencer != null)
            {
                sequencer.OnSlotChanged();
            }
        }
    }

    /// <summary>
    /// Start playing this slot's sound
    /// </summary>
    public void Play()
    {
        if (loopSource.clip != null)
        {
            loopSource.Play();
        }
    }

    /// <summary>
    /// Pause this slot's sound
    /// </summary>
    public void Pause()
    {
        if (loopSource.isPlaying)
        {
            loopSource.Pause();
        }
    }

    /// <summary>
    /// Resume this slot's sound
    /// </summary>
    public void Resume()
    {
        if (loopSource.clip != null)
        {
            loopSource.UnPause();
        }
    }

    /// <summary>
    /// Stop this slot's sound
    /// </summary>
    public void Stop()
    {
        loopSource.Stop();
    }

    /// <summary>
    /// Sync this slot to a specific time
    /// </summary>
    public void SyncToTime(float time)
    {
        if (loopSource.clip != null && loopSource.isPlaying)
        {
            loopSource.time = time % loopSource.clip.length;
        }
    }

    /// <summary>
    /// Update visual appearance based on state
    /// </summary>
    void UpdateVisuals()
    {
        if (backgroundImage != null)
        {
            if (currentSound != null)
            {
                backgroundImage.color = filledColor;
            }
            else
            {
                backgroundImage.color = emptyColor;
            }
        }
    }

    /// <summary>
    /// Highlight slot (when dragging over)
    /// </summary>
    public void Highlight(bool active)
    {
        if (backgroundImage != null)
        {
            if (active && currentSound == null)
            {
                backgroundImage.color = highlightColor;
            }
            else
            {
                UpdateVisuals();
            }
        }
    }
}