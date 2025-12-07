using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Makes a sound icon draggable for the mixing system
/// </summary>
public class DraggableSound : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] private CollectibleData soundData;

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Drag Settings")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private float dragAlpha = 0.6f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;
    private int originalSiblingIndex;

    // Current slot this sound is in (if any)
    private MixingSlot currentSlot;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Initialize with collectible data
    /// </summary>
    public void Setup(CollectibleData data)
    {
        soundData = data;

        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.displayName;
    }

    /// <summary>
    /// Get the audio clip
    /// </summary>
    public AudioClip GetAudioClip()
    {
        return soundData != null ? soundData.soundClip : null;
    }

    /// <summary>
    /// Get the collectible data
    /// </summary>
    public CollectibleData GetData()
    {
        return soundData;
    }

    /// <summary>
    /// Set which slot this sound is currently in
    /// </summary>
    public void SetCurrentSlot(MixingSlot slot)
    {
        currentSlot = slot;
    }

    /// <summary>
    /// Get current slot
    /// </summary>
    public MixingSlot GetCurrentSlot()
    {
        return currentSlot;
    }

    // ===== DRAG HANDLERS =====

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store original position
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalSiblingIndex = transform.GetSiblingIndex();

        // If we were in a slot, notify it
        if (currentSlot != null)
        {
            currentSlot.OnSoundRemovedByDrag(this);
        }

        // Move to canvas root for proper rendering over everything
        transform.SetParent(parentCanvas.transform);
        transform.SetAsLastSibling();

        // Make semi-transparent
        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow cursor
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore opacity and raycasts
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if dropped on a valid slot
        MixingSlot targetSlot = GetSlotUnderCursor(eventData);

        if (targetSlot != null && targetSlot.CanAcceptSound())
        {
            // Place in new slot
            targetSlot.PlaceSound(this);
        }
        else
        {
            // Return to original position
            ReturnToOriginalPosition();
        }
    }

    /// <summary>
    /// Single click to preview sound
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundData != null && soundData.soundClip != null)
        {
            // Load into preview player
            MixingPanelController panel = FindObjectOfType<MixingPanelController>();
            if (panel != null)
            {
                panel.PreviewSound(soundData.soundClip);
            }
        }
    }

    // ===== HELPER METHODS =====

    /// <summary>
    /// Find slot under cursor during drag
    /// </summary>
    MixingSlot GetSlotUnderCursor(PointerEventData eventData)
    {
        // Raycast to find what's under the cursor
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            MixingSlot slot = result.gameObject.GetComponent<MixingSlot>();
            if (slot != null)
                return slot;
        }

        return null;
    }

    /// <summary>
    /// Return to original parent and position
    /// </summary>
    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalSiblingIndex);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        currentSlot = null;
    }

    /// <summary>
    /// Place this sound in a specific parent transform (used by slots)
    /// </summary>
    public void PlaceInParent(Transform parent)
    {
        transform.SetParent(parent);
        rectTransform.anchoredPosition = Vector2.zero;
        transform.localScale = Vector3.one;
    }
}