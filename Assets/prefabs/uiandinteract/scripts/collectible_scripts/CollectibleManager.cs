using UnityEngine;
using System.Collections.Generic;
public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Database")]
    [SerializeField] private CollectibleDatabase database;

    [Header("Audio")]
    [SerializeField] private AudioClip defaultCollectionSound;

    // Events for UI updates
    public delegate void CollectibleCollectedHandler(CollectibleData data);
    public event CollectibleCollectedHandler OnCollectibleCollected;

    private Dictionary<string, CollectibleObject> registeredCollectibles = new Dictionary<string, CollectibleObject>();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Load save data and disable already collected items
        LoadCollectedStates();
    }

    /// <summary>
    /// Register a collectible object in the scene
    /// </summary>
    public void RegisterCollectible(CollectibleObject obj, string collectibleID)
    {
        if (string.IsNullOrEmpty(collectibleID))
        {
            Debug.LogWarning($"CollectibleManager: Empty ID for {obj.name}");
            return;
        }

        if (!database.HasCollectible(collectibleID))
        {
            Debug.LogWarning($"CollectibleManager: ID '{collectibleID}' not found in database");
            return;
        }

        if (registeredCollectibles.ContainsKey(collectibleID))
        {
            Debug.LogWarning($"CollectibleManager: Duplicate ID '{collectibleID}'");
            return;
        }

        registeredCollectibles.Add(collectibleID, obj);

        // Check if already collected
        if (SaveSystem.IsCollected(collectibleID))
        {
            obj.SetAlreadyCollected();
        }
    }

    /// <summary>
    /// Called when a collectible is collected
    /// </summary>
    public void OnCollect(string collectibleID, Vector3 position)
    {
        CollectibleData data = database.GetCollectibleByID(collectibleID);

        if (data == null)
        {
            Debug.LogError($"CollectibleManager: Data not found for ID '{collectibleID}'");
            return;
        }

        // Save progress
        SaveSystem.MarkCollected(collectibleID);

        // Play sound
        AudioClip clip = data.soundClip != null ? data.soundClip : defaultCollectionSound;
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAtPosition(clip, position, 0.8f);
        }

        // Show toast
        if (ToastManager.Instance != null)
        {
            ToastManager.Instance.Show($"Collected: {data.displayName}");
        }

        // Notify UI
        OnCollectibleCollected?.Invoke(data);

        Debug.Log($"✓ Collected: {data.displayName} ({collectibleID})");
    }

    /// <summary>
    /// Load collected states and disable glow for collected items
    /// </summary>
    private void LoadCollectedStates()
    {
        List<string> collectedIDs = SaveSystem.GetAllCollected();
        Debug.Log($"CollectibleManager: {collectedIDs.Count} items already collected");
    }

    /// <summary>
    /// Get collected count
    /// </summary>
    public int GetCollectedCount()
    {
        return SaveSystem.GetCollectedCount();
    }

    /// <summary>
    /// Get total collectibles
    /// </summary>
    public int GetTotalCount()
    {
        return database.allCollectibles.Count;
    }

    /// <summary>
    /// Debug: Clear all save data
    /// </summary>
    [ContextMenu("Clear All Save Data")]
    public void ClearAllSaveData()
    {
        SaveSystem.ClearAllData();
        Debug.Log("CollectibleManager: Save data cleared. Restart scene to see changes.");
    }
}