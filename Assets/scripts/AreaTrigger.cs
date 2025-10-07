using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
  [Header("Popup Settings")]
  public string areaName = "Rainforest";
  public bool showOnce = true;
  private bool hasShown = false;

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

    if (showOnce && hasShown)
    {
      Debug.Log($"AreaTrigger ({areaName}): Already shown, ignoring.");
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
      hasShown = true;
    }
    else
    {
      Debug.LogWarning($"AreaTrigger ({areaName}): AreaPopup reference is missing!");
    }
  }
}