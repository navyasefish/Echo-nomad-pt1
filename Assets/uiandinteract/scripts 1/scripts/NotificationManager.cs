using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("UI")]
    public RectTransform panel;
    public TextMeshProUGUI messageText;

    [Header("Settings")]
    public float slideSpeed = 1000f;
    public Vector2 onScreenPos = new Vector2(-20, 50);
    public Vector2 offScreenPos = new Vector2(400, 50);
    public float visibleTime = 2f;

    bool isShowing = false;

    void Awake()
    {
        Instance = this;
        panel.anchoredPosition = offScreenPos;
    }

    public void Show(string itemName)
    {
        StopAllCoroutines();
        StartCoroutine(NotificationRoutine(itemName));
    }

    IEnumerator NotificationRoutine(string itemName)
    {
        isShowing = true;
        messageText.text = $"Item Collected: {itemName}";

        // Slide In
        yield return StartCoroutine(Slide(panel, onScreenPos));

        // Stay visible
        yield return new WaitForSeconds(visibleTime);

        // Slide Out
        yield return StartCoroutine(Slide(panel, offScreenPos));

        isShowing = false;
    }

    IEnumerator Slide(RectTransform t, Vector2 target)
    {
        while (Vector2.Distance(t.anchoredPosition, target) > 0.5f)
        {
            t.anchoredPosition = Vector2.MoveTowards(
                t.anchoredPosition,
                target,
                slideSpeed * Time.unscaledDeltaTime
            );

            yield return null;
        }

        t.anchoredPosition = target;
    }
}
