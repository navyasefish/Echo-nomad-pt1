using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSound : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public SoundTileUI tile;
  private CanvasGroup canvasGroup;
  private Transform originalParent;
  private Vector3 originalLocalPosition;

  private void Awake()
  {
    canvasGroup = GetComponent<CanvasGroup>();
    if (canvasGroup == null)
      canvasGroup = gameObject.AddComponent<CanvasGroup>();
  }

  private void OnEnable()
  {
    originalParent = transform.parent;
    originalLocalPosition = transform.localPosition;
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (tile == null)
    {
      Debug.LogError("DraggableSound has no tile reference!");
      return;
    }

    if (!tile.CanUse())
    {
      Debug.Log($"Tile {tile.sound.soundID} cannot be used (already in a slot)");
      return;
    }

    Debug.Log($"Begin dragging {tile.sound.soundID}");

    // Store original state
    originalParent = transform.parent;
    originalLocalPosition = transform.localPosition;

    // Disable raycasts so drops work
    canvasGroup.blocksRaycasts = false;
    canvasGroup.alpha = 0.7f; // Visual feedback

    // Move to canvas root for proper layering
    Canvas canvas = GetComponentInParent<Canvas>();
    if (canvas != null)
      transform.SetParent(canvas.transform, true);
    else
      Debug.LogWarning("No Canvas found in parents!");
  }

  public void OnDrag(PointerEventData eventData)
  {
    if (tile == null || !tile.CanUse())
      return;

    // Follow mouse/touch position
    transform.position = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    Debug.Log($"End dragging {tile?.sound?.soundID ?? "unknown"}");

    // Re-enable raycasts
    canvasGroup.blocksRaycasts = true;
    canvasGroup.alpha = 1f;

    // Return to original position
    transform.SetParent(originalParent);
    transform.localPosition = originalLocalPosition;
  }
}