using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class WindNavigation : MonoBehaviour
{
  public WindZone Windzone;
  public Transform objective;
  public Transform player;
  public bool is_guiding = false;
  Vector3 direction = new Vector3(0f,0f,0f);
  float guide_ticks = 0f;
  float max_wait = 10f;
  void Update()
  {
    // Check for key press
    if (Input.GetKeyUp(KeyCode.Z))
    {
      Debug.Log("Z key pressed — attempting to start guiding wind.");
      start_guiding_wind();
    }

    if (is_guiding)
    {
      // Add guiding logic here later
      guide_ticks += Time.deltaTime;
      guide_ticks = guide_ticks > max_wait ? max_wait : guide_ticks;
      Debug.Log(" wind guide active !!"+ guide_ticks);
      Windzone.windMain = guide_ticks/2;
      if (guide_ticks == max_wait)
      {
        is_guiding = false;
        Debug.Log(" wind guide ended");
      }
    }
  }

  void start_guiding_wind()
  {
    if (is_guiding) return;

    // Start guiding
    direction = objective.position - player.position;
    Debug.DrawLine(player.position, objective.position, Color.green, 12f);
    Debug.Log($"Guiding wind started.\nDirection: {direction}\nFrom {player.name} to {objective.name}");
    Windzone.transform.rotation = Quaternion.LookRotation(direction);
    is_guiding = true;
    guide_ticks = 0f;
    Windzone.windMain = 0f;
  }
}
