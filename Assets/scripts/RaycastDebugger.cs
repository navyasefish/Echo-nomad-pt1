using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Attach to your Canvas to see what's blocking raycasts
/// </summary>
public class RaycastDebugger : MonoBehaviour
{
  public bool showDebugInfo = true;

  void Update()
  {
    if (!showDebugInfo)
      return;

    // Only check on mouse click/release
    if (Input.GetMouseButtonUp(0))
    {
      DebugRaycast(Input.mousePosition);
    }
  }

  void DebugRaycast(Vector2 screenPosition)
  {
    Debug.Log("===== RAYCAST DEBUG =====");
    Debug.Log($"Mouse position: {screenPosition}");

    // Get all raycasters
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    eventData.position = screenPosition;

    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);

    Debug.Log($"Total hits: {results.Count}");

    for (int i = 0; i < results.Count; i++)
    {
      RaycastResult result = results[i];
      GameObject go = result.gameObject;

      string info = $"[{i}] {go.name}";

      // Check components
      Image img = go.GetComponent<Image>();
      if (img != null)
        info += $" - Image (raycast: {img.raycastTarget}, enabled: {img.enabled})";

      DropSlot drop = go.GetComponent<DropSlot>();
      if (drop != null)
        info += " - ✅ HAS DROPSLOT";

      DraggableSound drag = go.GetComponent<DraggableSound>();
      if (drag != null)
        info += " - HAS DRAGGABLE";

      Debug.Log(info);
    }

    // Check specifically for DropSlots
    bool foundDropSlot = false;
    foreach (var result in results)
    {
      if (result.gameObject.GetComponent<DropSlot>() != null)
      {
        foundDropSlot = true;
        Debug.Log($"✅ DropSlot FOUND: {result.gameObject.name}");
        break;
      }
    }

    if (!foundDropSlot)
      Debug.LogWarning("❌ NO DROPSLOT in raycast results!");

    Debug.Log("========================");
  }

  // Visual debugging - draw red circle at mouse position
  void OnGUI()
  {
    if (!showDebugInfo)
      return;

    Vector2 mousePos = Input.mousePosition;
    mousePos.y = Screen.height - mousePos.y; // Flip Y for GUI

    GUI.color = Color.red;
    GUI.Box(new Rect(mousePos.x - 5, mousePos.y - 5, 10, 10), "");
  }
}