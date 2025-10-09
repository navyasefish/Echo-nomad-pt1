using UnityEngine;

public class ClimbingSparkleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem sparkleSystem;
    [SerializeField] private Renderer targetRenderer; // seaweed_2 renderer

    [Header("Climb Settings")]
    [SerializeField] private float climbDuration = 1.5f;
    [SerializeField] private AnimationCurve climbCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Material glowMaterial;
    private float climbProgress = 0f;
    private bool isClimbing = false;
    private bool isReversing = false;
    private static readonly int ClimbHeight = Shader.PropertyToID("_ClimbHeight");

    // For sparkle emission positioning
    private ParticleSystem.ShapeModule shapeModule;
    private float objectHeight;

    void Start()
    {
        // Get material instance
        if (targetRenderer != null)
        {
            glowMaterial = targetRenderer.material; // Creates instance automatically
        }

        // Setup particle system
        if (sparkleSystem != null)
        {
            shapeModule = sparkleSystem.shape;

            // Calculate object height
            Bounds bounds = targetRenderer.bounds;
            objectHeight = bounds.size.y;

            ConfigureSparkleSystem();
        }

        // Start with no glow
        SetClimbHeight(0f);
    }

    void ConfigureSparkleSystem()
    {
        var main = sparkleSystem.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 1f;
        main.startSize = 0.1f;
        main.maxParticles = 50;

        var emission = sparkleSystem.emission;
        emission.rateOverTime = 0; // We'll use bursts

        var shape = sparkleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f; // Adjust to seaweed width
        shape.position = Vector3.zero;

        var colorOverLifetime = sparkleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.yellow, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverLifetime.color = grad;
    }

    void Update()
    {
        if (isClimbing)
        {
            climbProgress += Time.deltaTime / climbDuration;

            if (climbProgress >= 1f)
            {
                climbProgress = 1f;
                isClimbing = false;
            }

            float easedProgress = climbCurve.Evaluate(climbProgress);
            SetClimbHeight(easedProgress);
            UpdateSparklePosition(easedProgress);
        }
        else if (isReversing)
        {
            climbProgress -= Time.deltaTime / climbDuration;

            if (climbProgress <= 0f)
            {
                climbProgress = 0f;
                isReversing = false;
                if (sparkleSystem != null)
                    sparkleSystem.Stop();
            }

            float easedProgress = climbCurve.Evaluate(climbProgress);
            SetClimbHeight(easedProgress);
        }
    }

    void SetClimbHeight(float height)
    {
        if (glowMaterial != null)
        {
            glowMaterial.SetFloat(ClimbHeight, height);
        }
    }

    void UpdateSparklePosition(float progress)
    {
        if (sparkleSystem != null && sparkleSystem.isPlaying)
        {
            // Move sparkle emission point up as glow climbs
            float yPosition = (progress - 0.5f) * objectHeight;

            var shape = sparkleSystem.shape;
            shape.position = new Vector3(0, yPosition, 0);

            // Emit burst of sparkles
            sparkleSystem.Emit(3);
        }
    }

    public void StartClimb()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            isReversing = false;
            climbProgress = 0f;

            if (sparkleSystem != null)
            {
                sparkleSystem.Clear();
                sparkleSystem.Play();
            }
        }
    }

    public void ReverseClimb()
    {
        if (!isReversing)
        {
            isReversing = true;
            isClimbing = false;
            // climbProgress stays where it is
        }
    }

    public void ResetGlow()
    {
        isClimbing = false;
        isReversing = false;
        climbProgress = 0f;
        SetClimbHeight(0f);

        if (sparkleSystem != null)
            sparkleSystem.Stop();
    }
}