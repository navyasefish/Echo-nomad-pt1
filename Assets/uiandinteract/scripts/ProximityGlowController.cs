using UnityEngine;

public class ProximityGlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ClimbingSparkleController climbController;

    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float exitRadius = 7f; // Slightly larger to prevent flickering

    private Transform playerTransform;
    private bool playerInRange = false;
    private bool isCollected = false;

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Auto-find climb controller if not assigned
        if (climbController == null)
            climbController = GetComponent<ClimbingSparkleController>();
    }

    void Update()
    {
        if (isCollected || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Player enters range
        if (!playerInRange && distance <= detectionRadius)
        {
            playerInRange = true;
            TriggerGlowClimb();
        }
        // Player exits range (with hysteresis)
        else if (playerInRange && distance > exitRadius)
        {
            playerInRange = false;
            TriggerGlowReverse();
        }
    }

    void TriggerGlowClimb()
    {
        if (climbController != null)
        {
            climbController.StartClimb();
        }
    }

    void TriggerGlowReverse()
    {
        if (climbController != null)
        {
            climbController.ReverseClimb();
        }
    }

    public void OnCollected()
    {
        isCollected = true;
        if (climbController != null)
        {
            climbController.ResetGlow();
        }
    }

    // Visualize ranges
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, exitRadius);
    }
}