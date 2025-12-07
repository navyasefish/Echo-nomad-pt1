using UnityEngine;

[CreateAssetMenu(menuName = "EchoNomad/SoundDefinition")]
public class SoundDefinition : ScriptableObject
{
  public string soundID;          // unique name (ex: "drum1", "melody2")
  public AudioClip clip;          // your loop audio
  public Sprite icon;             // for UI
}
