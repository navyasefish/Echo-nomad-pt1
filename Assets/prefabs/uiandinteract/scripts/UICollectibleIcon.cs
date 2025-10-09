using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UICollectibleIcon : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject lockedOverlay;

    [Header("Visual States")]
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;

    private CollectibleData data;
    private bool isUnlocked = false;

    void Awake()
    {
        // Setup button listener
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }
    }

    /// <summary>
    /// Initialize icon with collectible data
    /// </summary>
    public void Setup(CollectibleData collectibleData, bool unlocked)
    {
        data = collectibleData;
        isUnlocked = unlocked;

        // Set icon
        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
        }

        // Set name
        if (nameText != null)
        {
            nameText.text = data.displayName;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Unlock this icon
    /// </summary>
    public void Unlock()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        UpdateVisuals();

        // Optional: Play unlock animation
        StartCoroutine(UnlockAnimation());
    }

    void UpdateVisuals()
    {
        if (isUnlocked)
        {
            // Unlocked state
            if (iconImage != null)
                iconImage.color = unlockedColor;

            if (playButton != null)
                playButton.interactable = true;

            if (lockedOverlay != null)
                lockedOverlay.SetActive(false);
        }
        else
        {
            // Locked state
            if (iconImage != null)
                iconImage.color = lockedColor;

            if (playButton != null)
                playButton.interactable = false;

            if (lockedOverlay != null)
                lockedOverlay.SetActive(true);
        }
    }

    void OnPlayClicked()
    {
        if (!isUnlocked || data == null || data.soundClip == null) return;

        // Play sound through AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(data.soundClip, 0.7f);
        }

        Debug.Log($"Playing: {data.displayName}");
    }

    System.Collections.IEnumerator UnlockAnimation()
    {
        // Simple scale pop animation
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            yield return null;
        }

        // Scale back
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);

            yield return null;
        }

        transform.localScale = originalScale;
    }

    /// <summary>
    /// Get collectible ID
    /// </summary>
    public string GetCollectibleID()
    {
        return data != null ? data.collectibleID : "";
    }
}