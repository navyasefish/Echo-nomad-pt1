using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mini audio player for previewing sounds at the top of the mixing panel
/// </summary>
public class AudioPreviewPlayer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Image playPauseIcon;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Icons")]
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private bool isPlaying = false;
    private AudioClip currentClip;
    private bool isDraggingSlider = false;

    void Awake()
    {
        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlayPause);

        if (progressSlider != null)
        {
            progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
            progressSlider.value = 0f;
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;

        UpdatePlayPauseIcon(false);
        UpdateTimeText(0f, 0f);
    }

    void Update()
    {
        if (isPlaying && currentClip != null && audioSource.isPlaying)
        {
            // Update slider position while playing (unless user is dragging)
            if (!isDraggingSlider)
            {
                float normalizedTime = audioSource.time / currentClip.length;
                progressSlider.value = normalizedTime;
                UpdateTimeText(audioSource.time, currentClip.length);
            }

            // Check if audio finished
            if (!audioSource.isPlaying)
            {
                OnAudioFinished();
            }
        }
    }

    /// <summary>
    /// Load and prepare a clip for preview
    /// </summary>
    public void LoadClip(AudioClip clip)
    {
        Stop();

        currentClip = clip;
        audioSource.clip = clip;
        progressSlider.value = 0f;

        if (clip != null)
        {
            UpdateTimeText(0f, clip.length);
        }
        else
        {
            UpdateTimeText(0f, 0f);
        }
    }

    /// <summary>
    /// Toggle play/pause
    /// </summary>
    public void TogglePlayPause()
    {
        if (currentClip == null) return;

        if (isPlaying)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// Play from current position
    /// </summary>
    public void Play()
    {
        if (currentClip == null) return;

        audioSource.Play();
        isPlaying = true;
        UpdatePlayPauseIcon(true);
    }

    /// <summary>
    /// Pause at current position
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
        isPlaying = false;
        UpdatePlayPauseIcon(false);
    }

    /// <summary>
    /// Stop and reset
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
        progressSlider.value = 0f;
        UpdatePlayPauseIcon(false);

        if (currentClip != null)
            UpdateTimeText(0f, currentClip.length);
    }

    /// <summary>
    /// Called when audio finishes naturally
    /// </summary>
    void OnAudioFinished()
    {
        isPlaying = false;
        progressSlider.value = 1f;
        UpdatePlayPauseIcon(false);

        if (currentClip != null)
            UpdateTimeText(currentClip.length, currentClip.length);
    }

    /// <summary>
    /// Called when user drags the slider
    /// </summary>
    void OnSliderValueChanged(float value)
    {
        if (currentClip == null) return;

        // Only seek if user is actively dragging
        if (isDraggingSlider)
        {
            float targetTime = value * currentClip.length;
            audioSource.time = Mathf.Clamp(targetTime, 0f, currentClip.length);
            UpdateTimeText(audioSource.time, currentClip.length);
        }
    }

    /// <summary>
    /// Called when user starts dragging slider
    /// </summary>
    public void OnSliderDragStart()
    {
        isDraggingSlider = true;
    }

    /// <summary>
    /// Called when user stops dragging slider
    /// </summary>
    public void OnSliderDragEnd()
    {
        isDraggingSlider = false;
    }

    /// <summary>
    /// Update play/pause button icon
    /// </summary>
    void UpdatePlayPauseIcon(bool playing)
    {
        if (playPauseIcon != null)
        {
            playPauseIcon.sprite = playing ? pauseIcon : playIcon;
        }
    }

    /// <summary>
    /// Update time display text
    /// </summary>
    void UpdateTimeText(float current, float total)
    {
        if (timeText != null)
        {
            timeText.text = $"{FormatTime(current)} / {FormatTime(total)}";
        }
    }

    /// <summary>
    /// Format seconds to MM:SS
    /// </summary>
    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }
}