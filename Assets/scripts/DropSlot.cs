using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
  public SlotController slot;

  public void OnDrop(PointerEventData eventData)
  {
    DraggableSound dragged = eventData.pointerDrag?.GetComponent<DraggableSound>();
    if (dragged == null)
      return;

    SoundTileUI tile = dragged.tile;

    MixerUIManager mixer = FindAnyObjectByType<MixerUIManager>();

    // Already used?
    if (!tile.CanUse())
      return;

    mixer.DropSoundInSlot(tile.sound, slot);
  }
}
