using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UICollectiblePanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CollectibleDatabase database;
    [SerializeField] private Transform iconContainer; // Parent for icons (Grid Layout Group recommended)
    [SerializeField] private GameObject iconPrefab;

    [Header("Filter")]
    [SerializeField] private BiomeType currentBiomeFilter = BiomeType.Forest;

    private Dictionary<string, UICollectibleIcon> iconDictionary = new Dictionary<string, UICollectibleIcon>();

    void Start()
    {
        PopulateIcons();

        // Subscribe to collection events
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectibleCollected += OnCollectibleUnlocked;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectibleCollected -= OnCollectibleUnlocked;
        }
    }

    /// <summary>
    /// Populate all icons from database
    /// </summary>
    void PopulateIcons()
    {
        if (database == null || iconContainer == null || iconPrefab == null)
        {
            Debug.LogError("UICollectiblePanel: Missing references!");
            return;
        }

        // Get collectibles for current biome
        List<CollectibleData> collectibles = database.GetCollectiblesByBiome(currentBiomeFilter);

        foreach (CollectibleData data in collectibles)
        {
            // Instantiate icon
            GameObject iconObj = Instantiate(iconPrefab, iconContainer);
            UICollectibleIcon icon = iconObj.GetComponent<UICollectibleIcon>();

            if (icon == null)
            {
                Debug.LogError("UICollectiblePanel: Icon prefab missing UICollectibleIcon component!");
                continue;
            }

            // Check if already collected
            bool isUnlocked = SaveSystem.IsCollected(data.collectibleID);

            // Setup icon
            icon.Setup(data, isUnlocked);

            // Store reference
            iconDictionary.Add(data.collectibleID, icon);
        }

        Debug.Log($"UICollectiblePanel: Created {iconDictionary.Count} icons for {currentBiomeFilter} biome");
    }

    /// <summary>
    /// Called when a collectible is collected during gameplay
    /// </summary>
    void OnCollectibleUnlocked(CollectibleData data)
    {
        if (iconDictionary.ContainsKey(data.collectibleID))
        {
            iconDictionary[data.collectibleID].Unlock();
            Debug.Log($"UICollectiblePanel: Unlocked {data.displayName}");
        }
    }

    /// <summary>
    /// Change biome filter (for future multi-biome support)
    /// </summary>
    public void SetBiomeFilter(BiomeType biome)
    {
        currentBiomeFilter = biome;
        RefreshIcons();
    }

    /// <summary>
    /// Refresh all icons (useful after changing filter)
    /// </summary>
    void RefreshIcons()
    {
        // Clear existing icons
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }

        iconDictionary.Clear();

        // Repopulate
        PopulateIcons();
    }
}