using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectible", menuName = "Echo Nomad/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    [Header("Identity")]
    public string collectibleID; // Unique ID (e.g., "seaweed_forest_01")
    public string displayName; // Human-readable name

    [Header("Audio")]
    public AudioClip soundClip;

    [Header("Visual")]
    public Sprite icon; // For UI display
    public Sprite lockedIcon; // Greyed out version (optional)

    [Header("Description")]
    [TextArea(3, 6)]
    public string description;

    [Header("Biome Classification")]
    public BiomeType biome = BiomeType.Forest;
}

public enum BiomeType
{
    Forest,
    Desert,
    Mountain,
    Cave,
    Other
}