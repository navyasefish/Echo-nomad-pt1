using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindNavigation : MonoBehaviour
{
  public WindZone Windzone;
  public Transform objective;
  public Transform player;
  public bool is_guiding = false;

  Vector3 direction = Vector3.zero;
  public float windMax = 5f;       // Maximum wind strength
  public float windSpeed = 1f;     // How fast wind increases/decreases
  private float currentWind = 0f;

  void Update()
  {
    // Toggle guiding on Z key press
    if (Input.GetKeyUp(KeyCode.Z))
    {
      is_guiding = !is_guiding;
      Debug.Log(is_guiding ? "✅ Guiding wind started." : "🛑 Guiding wind stopped.");
    }

    // Update wind direction and strength
    if (is_guiding)
    {
      // Update direction every frame
      direction = objective.position - player.position;
      direction.y = 0f; // optional: keep wind horizontal
      Windzone.transform.rotation = Quaternion.LookRotation(direction);

      // Smoothly increase wind
      currentWind = Mathf.Min(currentWind + windSpeed * Time.deltaTime, windMax);
    }
    else
    {
      // Smoothly decrease wind back to zero
      currentWind = Mathf.Max(currentWind - windSpeed * Time.deltaTime, 0f);
    }

    Windzone.windMain = currentWind;

    // Optional debug
    Debug.DrawLine(player.position, objective.position, Color.cyan);
    //Debug.Log($"Wind forward: {Windzone.transform.forward}, Strength: {currentWind}");
  }
}
