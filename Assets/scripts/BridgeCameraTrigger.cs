using UnityEngine;
using Cinemachine;

public class BridgeCameraTrigger : MonoBehaviour
{
  public CinemachineVirtualCamera bridgeCamera;

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      bridgeCamera.Priority = 20; // activate bridge camera
    }
  }

  void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      bridgeCamera.Priority = 9; // revert to default camera
    }
  }
}
