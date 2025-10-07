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

  private void OnTriggerEnter(Collider other)
  {
    Debug.Log($"AreaTrigger: OnTriggerEnter called. Collider = {other.name}");

    if (!other.CompareTag("Player"))
    {
      Debug.Log("AreaTrigger: Collider is not the player, ignoring.");
      return;
    }

    if (showOnce && hasShown)
    {
      Debug.Log("AreaTrigger: Already shown, ignoring.");
      return;
    }

    if (oneSided)
    {
      Vector3 toPlayer = other.transform.position - transform.position;
      float dot = Vector3.Dot(toPlayer.normalized, transform.TransformDirection(triggerForward));
      Debug.Log($"AreaTrigger: Dot product = {dot}");

      if (dot < 0)
      {
        Debug.Log($"AreaTrigger: Player entered from front, showing popup '{areaName}'");
        FindObjectOfType<AreaPopup>().ShowAreaName(areaName);
        hasShown = true;
      }
      else
      {
        Debug.Log("AreaTrigger: Player entered from the wrong side, ignoring.");
      }
    }
    else
    {
      Debug.Log($"AreaTrigger: Showing popup '{areaName}' (no direction check)");
      FindObjectOfType<AreaPopup>().ShowAreaName(areaName);
      hasShown = true;
    }
  }
}
