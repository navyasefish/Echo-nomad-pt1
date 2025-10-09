using UnityEngine;

public class ProximityGlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material glowMaterial; // Assign your glow material
    [SerializeField] private Renderer targetRenderer; // Assign seaweed_2's renderer

    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer; // Set to Player layer

    [Header("Glow Settings")]
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float fadeSpeed = 2f;

    private Transform playerTransform;
    private float currentGlowStrength = 0f;
    private bool isCollected = false;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Color baseEmissionColor;

    void Start()
    {
        // Find player (adjust tag if needed)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Store base emission color
        if (glowMaterial != null)
            baseEmissionColor = glowMaterial.GetColor(EmissionColor);

        // Start with glow off
        SetGlowStrength(0f);
    }

    void Update()
    {
        if (isCollected || playerTransform == null) return;

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Determine target glow strength
        float targetGlow = distance <= detectionRadius ? 1f : 0f;

        // Smoothly transition glow
        currentGlowStrength = Mathf.Lerp(currentGlowStrength, targetGlow, Time.deltaTime * fadeSpeed);
        SetGlowStrength(currentGlowStrength);
    }

    private void SetGlowStrength(float strength)
    {
        if (glowMaterial == null) return;

        Color emissionColor = baseEmissionColor * (strength * glowIntensity);
        glowMaterial.SetColor(EmissionColor, emissionColor);

        // Enable/disable emission keyword
        if (strength > 0.01f)
            glowMaterial.EnableKeyword("_EMISSION");
        else
            glowMaterial.DisableKeyword("_EMISSION");
    }

    public void OnCollected()
    {
        isCollected = true;
        SetGlowStrength(0f); // Turn off glow permanently
    }

    // Visualize detection radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}