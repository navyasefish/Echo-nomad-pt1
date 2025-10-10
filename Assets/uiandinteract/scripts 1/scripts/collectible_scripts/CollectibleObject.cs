using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollectibleObject : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip collectionSound;
    [SerializeField] private float soundVolume = 1f;

    [Header("Collection Settings")]
    [SerializeField] private bool autoCollectOnProximity = false;
    [SerializeField] private float autoCollectRadius = 1.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("References")]
    [SerializeField] private ProximityGlowController glowController;

    [Header("Data")]
    [SerializeField] private string collectibleID; // Unique ID matching database

    private bool isCollected = false;
    private Transform playerTransform;
    private bool playerInRange = false;

    private AudioSource audioSource;

    void Start()
    {
        // Get AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Register with manager
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.RegisterCollectible(this, collectibleID);
        }

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Auto-find glow controller if not assigned
        if (glowController == null)
            glowController = GetComponent<ProximityGlowController>();
    }

    void Update()
    {
        if (isCollected || playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        playerInRange = distanceToPlayer <= autoCollectRadius;

        // Auto-collect if enabled
        if (autoCollectOnProximity && playerInRange)
        {
            Collect();
        }
        // Manual collection with key press
        else if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Collect();
        }
    }

    void Collect()
    {
        if (isCollected) return;

        isCollected = true;

        // Play collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound, soundVolume);
        }

        // Turn off glow
        if (glowController != null)
        {
            glowController.OnCollected();
        }

        // Notify the manager (this will trigger saving, UI, etc.)
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollect(this.collectibleID, transform.position);
        }

        Debug.Log($"Collected: {gameObject.name}");
    }

    /// <summary>
    /// Called by CollectibleManager to mark this as already collected on load.
    /// Disables glow and interaction.
    /// </summary>
    public void SetAlreadyCollected()
    {
        isCollected = true;

        // Turn off glow
        if (glowController != null)
        {
            glowController.OnCollected();
        }

        // Optionally disable collider to prevent interaction
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Optionally change appearance to mark as collected
        //Renderer renderer = GetComponent<Renderer>();
        //if (renderer != null) renderer.material.color = Color.gray;

        Debug.Log($"SetAlreadyCollected: {gameObject.name} ({collectibleID})");
    }

    // Visualize collection radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoCollectRadius);
    }
}
