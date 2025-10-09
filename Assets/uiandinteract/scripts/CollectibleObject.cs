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

    private AudioSource audioSource;
    private bool isCollected = false;
    private Transform playerTransform;
    private bool playerInRange = false;

    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.maxDistance = 20f;

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
        if (collectionSound != null)
        {
            audioSource.PlayOneShot(collectionSound, soundVolume);
        }

        // Turn off glow
        if (glowController != null)
        {
            glowController.OnCollected();
        }

        // Keep object visible but mark as collected
        Debug.Log($"Collected: {gameObject.name}");

        // TODO: In Stage 5, we'll notify CollectibleManager here
        // CollectibleManager.Instance.OnCollect(this.collectibleID);
    }

    // Visualize collection radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoCollectRadius);
    }
}