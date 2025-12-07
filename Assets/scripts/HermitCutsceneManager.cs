using UnityEngine;
using System.Collections;

public class HermitCutsceneManager : MonoBehaviour, IStartConversation
{
  [Header("UI")]
  [Tooltip("Root container for the cutscene UI")]
  public GameObject cutsceneRoot;

  [Tooltip("Ordered list of panels (children) for each page in the cutscene")]
  public GameObject[] panels;

  [Header("Audio")]
  public AudioSource cutsceneBgAudio;

  [Header("Settings")]
  public float autoAdvanceDelay = 0f; // 0 => no auto advance; >0 => waits then advances

  // internal
  private int index = 0;
  private HermitInteractionController caller;

  private void Start()
  {
    if (cutsceneRoot != null) cutsceneRoot.SetActive(false);
    foreach (var p in panels) if (p != null) p.SetActive(false);
  }

  public void StartConversation(HermitInteractionController caller)
  {
    this.caller = caller;
    index = 0;
    if (cutsceneRoot != null) cutsceneRoot.SetActive(true);
    ShowPanel(index);

    if (cutsceneBgAudio != null) cutsceneBgAudio.Play();

    // start listening for Space -- Update handles it
  }

  private void Update()
  {
    if (cutsceneRoot == null || !cutsceneRoot.activeSelf) return;

    if (Input.GetKeyDown(KeyCode.Space))
    {
      NextPanel();
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

    // callback to caller to restore movement and optionally disable blocker
    caller?.EndInteraction(disableBlocker: true, resumeHumming: false);
  }
}
