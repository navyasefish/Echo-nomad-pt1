using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
  public SlotController slot;

  private void Awake()
  {
    // CRITICAL: Ensure we have an Image for raycasting
    Image img = GetComponent<Image>();
    if (img == null)
    {
      Debug.LogWarning($"{gameObject.name}: Adding Image component for drop detection");
      img = gameObject.AddComponent<Image>();
      img.color = new Color(1, 1, 1, 0.02f); // Nearly invisible
    }

    // CRITICAL: Raycast target MUST be enabled
    if (!img.raycastTarget)
    {
      Debug.LogWarning($"{gameObject.name}: Enabling raycastTarget on Image");
      img.raycastTarget = true;
    }

    Debug.Log($"✅ DropSlot on {gameObject.name} initialized (raycastTarget: {img.raycastTarget})");
  }

  public void OnDrop(PointerEventData eventData)
  {
    Debug.Log($"🎯 OnDrop CALLED on {gameObject.name}!");
    Debug.Log($"  - pointerDrag: {eventData.pointerDrag?.name}");

    if (eventData.pointerDrag == null)
    {
      Debug.LogError("  ❌ pointerDrag is NULL!");
      return;
    }

    DraggableSound dragged = eventData.pointerDrag.GetComponent<DraggableSound>();
    if (dragged == null)
    {
      Debug.LogError($"  ❌ No DraggableSound on {eventData.pointerDrag.name}");
      return;
    }

    Debug.Log($"  ✅ Found DraggableSound");

    SoundTileUI tile = dragged.tile;
    if (tile == null)
    {
      Debug.LogError("  ❌ DraggableSound.tile is NULL");
      return;
    }

    Debug.Log($"  ✅ Found tile with sound: {tile.sound?.soundID}");

    if (tile.sound == null)
    {
      Debug.LogError("  ❌ SoundTileUI.sound is NULL");
      return;
    }

    MixerUIManager mixer = FindAnyObjectByType<MixerUIManager>();
    if (mixer == null)
    {
      Debug.LogError("  ❌ MixerUIManager not found!");
      return;
    }

    // Check if already used
    if (!tile.CanUse())
    {
      Debug.LogWarning($"  ⚠️ Sound {tile.sound.soundID} already in use");
      return;
    }

    Debug.Log($"  ➡️ Calling mixer.DropSoundInSlot for {tile.sound.soundID}");
    mixer.DropSoundInSlot(tile.sound, slot);
  }
}