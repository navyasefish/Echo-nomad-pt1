using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
  [Header("Popup Settings")]
  public string areaName = "Rainforest";
  public bool showOnce = false;
  public float cooldownTime = 5f;
  private float lastShownTime = -999f;

  [Header("Direction Settings")]
  public bool oneSided = true;
  public Vector3 triggerForward = Vector3.forward;

  [Header("References")]
  public AreaPopup areaPopup; // Assign your AreaPopup here

  private void OnTriggerEnter(Collider other)
  {
    Debug.Log($"AreaTrigger ({areaName}): OnTriggerEnter called. Collider = {other.name}");

    if (!other.CompareTag("Player"))
    {
      Debug.Log($"AreaTrigger ({areaName}): Collider is not the player, ignoring.");
      return;
    }

    if (showOnce && lastShownTime > 0)
    {
      Debug.Log($"AreaTrigger ({areaName}): Already shown once, ignoring.");
      return;
    }

    if (!showOnce && (Time.time - lastShownTime) < cooldownTime)
    {
      float remaining = cooldownTime - (Time.time - lastShownTime);
      Debug.Log($"AreaTrigger ({areaName}): On cooldown. {remaining:F1}s remaining.");
      return;
    }

    if (oneSided)
    {
      Vector3 toPlayer = other.transform.position - transform.position;
      float dot = Vector3.Dot(toPlayer.normalized, transform.TransformDirection(triggerForward));
      Debug.Log($"AreaTrigger ({areaName}): Dot product = {dot}");

      if (dot < 0)
      {
        Debug.Log($"AreaTrigger ({areaName}): Player entered from front, showing popup");
        ShowPopup();
      }
      else
      {
        Debug.Log($"AreaTrigger ({areaName}): Player entered from wrong side, ignoring.");
      }
    }
    else
    {
      Debug.Log($"AreaTrigger ({areaName}): Showing popup (no direction check)");
      ShowPopup();
    }
  }

  private void ShowPopup()
  {
    if (areaPopup != null)
    {
      areaPopup.ShowAreaName(areaName);
      lastShownTime = Time.time;
    }
    else
    {
      Debug.LogWarning($"AreaTrigger ({areaName}): AreaPopup reference is missing!");
    }
  }
}