using UnityEngine;
using System.Collections;

public class HermitCutsceneManager : MonoBehaviour, IStartConversation
{
  [Header("UI")]
  [Tooltip("Root container for the cutscene UI")]
  public GameObject cutsceneRoot;

  [Tooltip("Ordered list of panels (children) for each page in the cutscene")]
  public GameObject[] panels;

  [Tooltip("Panel shown on repeated interactions (after first time)")]
  public GameObject repeatedCutscenePanel;

  [Header("Audio")]
  public AudioSource cutsceneBgAudio;

  [Header("Settings")]
  public float autoAdvanceDelay = 0f; // 0 => no auto advance; >0 => waits then advances

  // internal
  private int index = 0;
  private HermitInteractionController caller;
  private bool isFirstInteraction = true;
  private bool isRepeatedMode = false;

  private void Start()
  {
    if (cutsceneRoot != null) cutsceneRoot.SetActive(false);
    foreach (var p in panels) if (p != null) p.SetActive(false);
    if (repeatedCutscenePanel != null) repeatedCutscenePanel.SetActive(false);
  }

  public void StartConversation(HermitInteractionController caller)
  {
    this.caller = caller;

    if (cutsceneRoot != null) cutsceneRoot.SetActive(true);

    if (isFirstInteraction)
    {
      // First time: show full cutscene
      isRepeatedMode = false;
      index = 0;
      ShowPanel(index);
      if (cutsceneBgAudio != null) cutsceneBgAudio.Play();
    }
    else
    {
      // Subsequent times: show only repeated panel
      isRepeatedMode = true;
      if (repeatedCutscenePanel != null) repeatedCutscenePanel.SetActive(true);
      if (cutsceneBgAudio != null) cutsceneBgAudio.Play();
    }
  }

  private void Update()
  {
    if (cutsceneRoot == null || !cutsceneRoot.activeSelf) return;

    // Handle Left Mouse Click to advance
    if (Input.GetMouseButtonDown(0))
    {
      if (isRepeatedMode)
      {
        // Close repeated panel immediately
        EndCutscene();
      }
      else
      {
        NextPanel();
      }
    }
  }

  private void NextPanel()
  {
    index++;
    if (index < panels.Length)
    {
      ShowPanel(index);
      if (autoAdvanceDelay > 0f)
      {
        StartCoroutine(AutoAdvanceCoroutine());
      }
    }
    else
    {
      // First cutscene completed
      isFirstInteraction = false;
      EndCutscene();
    }
  }

  private IEnumerator AutoAdvanceCoroutine()
  {
    yield return new WaitForSeconds(autoAdvanceDelay);
    NextPanel();
  }

  private void ShowPanel(int i)
  {
    for (int k = 0; k < panels.Length; k++)
    {
      if (panels[k] != null) panels[k].SetActive(k == i);
    }
  }

  private void EndCutscene()
  {
    if (cutsceneBgAudio != null) cutsceneBgAudio.Stop();
    if (cutsceneRoot != null) cutsceneRoot.SetActive(false);
    if (repeatedCutscenePanel != null) repeatedCutscenePanel.SetActive(false);

    // callback to caller to restore movement and optionally disable blocker
    caller?.EndInteraction(disableBlocker: true, resumeHumming: false);
  }
}