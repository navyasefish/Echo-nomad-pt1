using UnityEngine;
using UnityEngine.UI;

public class SoundTileUI : MonoBehaviour
{
  public SoundDefinition sound; // Drag your ScriptableObject here
  public Image iconImage;       // The icon display

  void Start()
  {
    // Set the icon from the sound definition
    if (sound != null && iconImage != null)
    {
      iconImage.sprite = sound.icon;
    }
  }
}