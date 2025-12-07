using UnityEngine;

[DisallowMultipleComponent]
public class PlayerMovementBridge : MonoBehaviour
{
  [Tooltip("Optional: a reference to your movement component (drag your movement script here). If assigned, this component will be enabled/disabled.")]
  public MonoBehaviour movementComponent;

  private bool disabled = false;
  private CharacterController charController;
  private Rigidbody rb;

  private void Awake()
  {
    if (movementComponent == null)
    {
      // fallback detection
      charController = GetComponent<CharacterController>();
      rb = GetComponent<Rigidbody>();
    }
  }

  public void DisableMovement()
  {
    if (disabled) return;

    if (movementComponent != null)
    {
      movementComponent.enabled = false;
    }
    else
    {
      if (charController != null) charController.enabled = false;
      if (rb != null) rb.isKinematic = true;
    }

    disabled = true;
  }

  public void EnableMovement()
  {
    if (!disabled) return;

    if (movementComponent != null)
    {
      movementComponent.enabled = true;
    }
    else
    {
      if (charController != null) charController.enabled = true;
      if (rb != null) rb.isKinematic = false;
    }

    disabled = false;
  }
}
