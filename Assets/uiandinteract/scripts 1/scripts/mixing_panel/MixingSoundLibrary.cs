using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Displays all unlocked collectible sounds as draggable tiles in a scrollable library
/// </summary>
public class MixingSoundLibrary : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CollectibleDatabase database;
    [SerializeField] private Transform contentContainer; // ScrollView > Viewport > Content
    [SerializeField] private GameObject draggableSoundPrefab;

    [Header("Filter (Optional)")]
    [SerializeField] private bool filterByBiome = false;
    [SerializeField] private BiomeType currentBiome = BiomeType.Forest;

    private Dictionary<string, DraggableSound> soundTiles = new Dictionary<string, DraggableSound>();

    void Start()
    {
        PopulateLibrary();

        // Subscribe to collection events to add new sounds in real-time
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectibleCollected += OnNewSoundUnlocked;
        }
    }

    void OnDestroy()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectibleCollected -= OnNewSoundUnlocked;
        }
    }

    /// <summary>
    /// Populate library with all unlocked sounds
    /// </summary>
    void PopulateLibrary()
    {
        if (database == null || contentContainer == null || draggableSoundPrefab == null)
        {
            Debug.LogError("MixingSoundLibrary: Missing references!");
            return;
        }

        // Get collectibles (filtered or all)
        List<CollectibleData> collectibles;
        if (filterByBiome)
        {
            collectibles = database.GetCollectiblesByBiome(currentBiome);
        }
        else
        {
            collectibles = database.allCollectibles;
        }

        // Create draggable tile for each unlocked collectible
        foreach (var data in collectibles)
        {
            // Check if unlocked
            if (!SaveSystem.IsCollected(data.collectibleID))
                continue;

            CreateSoundTile(data);
        }

        Debug.Log($"MixingSoundLibrary: Populated with {soundTiles.Count} unlocked sounds");
    }

    /// <summary>
    /// Create a draggable sound tile
    /// </summary>
    void CreateSoundTile(CollectibleData data)
    {
        // Don't create duplicates
        if (soundTiles.ContainsKey(data.collectibleID))
            return;

        // Instantiate tile
        GameObject tileObj = Instantiate(draggableSoundPrefab, contentContainer);
        DraggableSound tile = tileObj.GetComponent<DraggableSound>();

        if (tile == null)
        {
            Debug.LogError("MixingSoundLibrary: Prefab missing DraggableSound component!");
            Destroy(tileObj);
            return;
        }

        // Setup tile
        tile.Setup(data);

        // Store reference
        soundTiles.Add(data.collectibleID, tile);
    }

    /// <summary>
    /// Called when a new sound is unlocked during gameplay
    /// </summary>
    void OnNewSoundUnlocked(CollectibleData data)
    {
        // If filtering by biome, check if this sound belongs to current biome
        if (filterByBiome && data.biome != currentBiome)
            return;

        // Add to library if not already present
        if (!soundTiles.ContainsKey(data.collectibleID))
        {
            CreateSoundTile(data);
            Debug.Log($"MixingSoundLibrary: Added new sound - {data.displayName}");
        }
    }

    /// <summary>
    /// Change biome filter
    /// </summary>
    public void SetBiomeFilter(BiomeType biome)
    {
        if (!filterByBiome) return;

        currentBiome = biome;
        RefreshLibrary();
    }

    /// <summary>
    /// Refresh entire library (clear and repopulate)
    /// </summary>
    void RefreshLibrary()
    {
        // Clear existing tiles
        foreach (var tile in soundTiles.Values)
        {
            if (tile != null)
                Destroy(tile.gameObject);
        }

        soundTiles.Clear();

        // Repopulate
        PopulateLibrary();
    }

    /// <summary>
    /// Get count of unlocked sounds in library
    /// </summary>
    public int GetUnlockedCount()
    {
        return soundTiles.Count;
    }
}