using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CollectibleDatabase", menuName = "Echo Nomad/Collectible Database")]
public class CollectibleDatabase : ScriptableObject
{
    [Header("All Collectibles")]
    public List<CollectibleData> allCollectibles = new List<CollectibleData>();

    /// <summary>
    /// Get collectible by ID
    /// </summary>
    public CollectibleData GetCollectibleByID(string id)
    {
        return allCollectibles.FirstOrDefault(c => c.collectibleID == id);
    }

    /// <summary>
    /// Get all collectibles from a specific biome
    /// </summary>
    public List<CollectibleData> GetCollectiblesByBiome(BiomeType biome)
    {
        return allCollectibles.Where(c => c.biome == biome).ToList();
    }

    /// <summary>
    /// Check if collectible exists
    /// </summary>
    public bool HasCollectible(string id)
    {
        return allCollectibles.Any(c => c.collectibleID == id);
    }
}