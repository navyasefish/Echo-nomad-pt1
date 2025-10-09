using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup toastCanvasGroup;
    [SerializeField] private TextMeshProUGUI toastText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float slideDistance = 50f;

    private RectTransform toastRect;
    private Vector3 originalPosition;
    private Coroutine currentToastCoroutine;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        toastRect = toastCanvasGroup.GetComponent<RectTransform>();
        originalPosition = toastRect.localPosition;

        // Start invisible
        toastCanvasGroup.alpha = 0f;
        toastCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// Display a toast notification
    /// </summary>
    public void Show(string message)
    {
        // Stop previous toast if running
        if (currentToastCoroutine != null)
        {
            StopCoroutine(currentToastCoroutine);
        }

        currentToastCoroutine = StartCoroutine(ShowToastCoroutine(message));
    }

    IEnumerator ShowToastCoroutine(string message)
    {
        // Setup
        toastText.text = message;
        toastCanvasGroup.gameObject.SetActive(true);
        toastCanvasGroup.alpha = 0f;

        // Start below original position
        Vector3 startPos = originalPosition - new Vector3(0, slideDistance, 0);
        toastRect.localPosition = startPos;

        // Fade in + slide up
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;

            toastCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            toastRect.localPosition = Vector3.Lerp(startPos, originalPosition, t);

            yield return null;
        }

        toastCanvasGroup.alpha = 1f;
        toastRect.localPosition = originalPosition;

        // Display
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            toastCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        toastCanvasGroup.alpha = 0f;
        toastCanvasGroup.gameObject.SetActive(false);

        currentToastCoroutine = null;
    }
}