using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSound : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public SoundTileUI tile;
  private CanvasGroup canvasGroup;
  private Transform originalParent;

  void Start()
  {
    canvasGroup = gameObject.AddComponent<CanvasGroup>();
    originalParent = transform.parent;
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (!tile.CanUse())
      return;

    canvasGroup.blocksRaycasts = false;
    transform.SetParent(GameObject.Find("Canvas").transform);
  }

  public void OnDrag(PointerEventData eventData)
  {
    if (!tile.CanUse())
      return;

    transform.position = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    canvasGroup.blocksRaycasts = true;
    transform.SetParent(originalParent);
    transform.localPosition = Vector3.zero;
  }
}
