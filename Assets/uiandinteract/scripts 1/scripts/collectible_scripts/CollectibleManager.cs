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

    private Dictionary<string, List<CollectibleObject>> registeredCollectibles = new Dictionary<string, List<CollectibleObject>>();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        // ⭐ Reset save data on new build version
        SaveSystem.CheckVersion();
    }

    void Start()
    {
        LoadCollectedStates();
    }

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

        if (!registeredCollectibles.ContainsKey(collectibleID))
        {
            registeredCollectibles[collectibleID] = new List<CollectibleObject>();
        }

        registeredCollectibles[collectibleID].Add(obj);

        if (SaveSystem.IsCollected(collectibleID))
        {
            obj.SetAlreadyCollected();
        }
    }

    public void OnCollect(string collectibleID, Vector3 position)
    {
        CollectibleData data = database.GetCollectibleByID(collectibleID);

        if (data == null)
        {
            Debug.LogError($"CollectibleManager: Data not found for ID '{collectibleID}'");
            return;
        }

        // Save
        SaveSystem.MarkCollected(collectibleID);

        // Play sound
        AudioClip clip = data.soundClip != null ? data.soundClip : defaultCollectionSound;
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAtPosition(clip, position, 0.3f);
        }

        // ⭐ NEW Notification Panel (2 sec later)
        StartCoroutine(ShowDelayedNotification(data.displayName));

        // Update UI
        OnCollectibleCollected?.Invoke(data);

        Debug.Log($"✓ Collected: {data.displayName} ({collectibleID})");
    }

    private System.Collections.IEnumerator ShowDelayedNotification(string itemName)
    {
        yield return new WaitForSeconds(2f);

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.Show(itemName);
        }
    }

    private void LoadCollectedStates()
    {
        List<string> collectedIDs = SaveSystem.GetAllCollected();
        Debug.Log($"CollectibleManager: {collectedIDs.Count} items already collected");
    }

    public int GetCollectedCount()
    {
        return SaveSystem.GetCollectedCount();
    }

    public int GetTotalCount()
    {
        return database.allCollectibles.Count;
    }

    [ContextMenu("Clear All Save Data")]
    public void ClearAllSaveData()
    {
        SaveSystem.ClearAllData();
        Debug.Log("CollectibleManager: Save data cleared.");
    }
}
