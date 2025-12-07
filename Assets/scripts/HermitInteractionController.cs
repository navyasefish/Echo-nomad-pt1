using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HermitInteractionController : MonoBehaviour
{
  [Tooltip("Mixer panel UI")]
  public GameObject mixerPanel;

  [Header("References (assign in inspector)")]
  [Tooltip("The Hermit object (for audio, optionally animations)")]
  public GameObject hermitObject;

  [Tooltip("Hermit audio controller (on Hermit or child)")]
  public HermitAudioController hermitAudio;

  [Tooltip("Physical blocker GameObject (non-trigger collider) that blocks path. Must be active to block.")]
  public GameObject blockerObject;

  [Header("UI Panel (single parent)")]
  [Tooltip("Parent UI panel that contains InteractOption and MixerOption as children")]
  public GameObject optionPanel;


  [Tooltip("Child button or UI for Interact (E)")]
  public GameObject interactOption;

  [Tooltip("Child button or UI for Mixer (M)")]
  public GameObject mixerOption;

  [Header("Player")]
  [Tooltip("Assign the Player GameObject (tag must be 'Player')")]
  public GameObject player;

  [Tooltip("Optional: PlayerMovementBridge on player (recommended). If not set, script will attempt CharacterController/Rigidbody toggles.")]
  public PlayerMovementBridge playerBridge;

  [Header("Cutscene Manager")]
  [Tooltip("Assign a component that implements IStartConversation (e.g., HermitCutsceneManager).")]
  public MonoBehaviour cutsceneManager;

  [Header("Input")]
  public KeyCode interactKey = KeyCode.E;
  public KeyCode mixerKey = KeyCode.M;

  [Header("Events")]
  public UnityEvent OnBeginConversation;
  public UnityEvent OnEndConversation;

  // internal
  private bool playerInRange = false;
  private bool initialCutsceneDone = false;
  private bool conversationActive = false;

  private void Reset()
  {
    // try auto-assign collider if on same object
    var col = GetComponent<Collider>();
    if (col != null)
      col.isTrigger = true;
  }

  private void Start()
  {
    if (optionPanel != null) optionPanel.SetActive(false);
    if (interactOption != null) interactOption.SetActive(true);
    if (mixerOption != null) mixerOption.SetActive(false);
    if (blockerObject != null) blockerObject.SetActive(true);
    if (mixerPanel != null) mixerPanel.SetActive(false);


    // auto-assign playerBridge if not set
    if (player != null && playerBridge == null)
      playerBridge = player.GetComponent<PlayerMovementBridge>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("Player")) return;

    playerInRange = true;
    ShowOptions();
    hermitAudio?.FadeInHumming();
  }

  private void OnTriggerExit(Collider other)
  {
    if (!other.CompareTag("Player")) return;

    playerInRange = false;
    optionPanel?.SetActive(false);
    hermitAudio?.FadeOutHumming();
  }

  private void Update()
  {
    if (!playerInRange || conversationActive) return;

    if (Input.GetKeyDown(interactKey))
    {
      StartInteraction();
    }
    // IMPORTANT: This should ONLY trigger on M key, not E
    else if (initialCutsceneDone && Input.GetKeyDown(mixerKey))
    {
      OpenMixer();
    }
  }

  private void ShowOptions()
  {
    if (optionPanel == null) return;

    optionPanel.SetActive(true);
    if (interactOption != null) interactOption.SetActive(true);
    if (mixerOption != null) mixerOption.SetActive(initialCutsceneDone);
  }

  private void StartInteraction()
  {
    conversationActive = true;

    // hide UI prompt
    optionPanel?.SetActive(false);

    // fade out hermit ambient quickly
    hermitAudio?.FadeOutHumming();

    // freeze player
    if (playerBridge != null)
    {
      playerBridge.DisableMovement();
      // Also disable jump if your bridge has this method
      // playerBridge.DisableJump();
    }
    else
    {
      // fallback: attempt CharacterController/Rigidbody
      var cc = player?.GetComponent<CharacterController>();
      if (cc != null) cc.enabled = false;
      var rb = player?.GetComponent<Rigidbody>();
      if (rb != null)
      {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero; // Stop any movement
      }
    }

    // keep blocker active (blocks passage). If you prefer to only enable when interacting, enable it here.
    if (blockerObject != null) blockerObject.SetActive(true);

    OnBeginConversation?.Invoke();

    // Start cutscene via interface if possible
    if (cutsceneManager != null)
    {
      var conv = cutsceneManager as IStartConversation;
      if (conv != null)
      {
        conv.StartConversation(this);
      }
      else
      {
        // fallback: try invoke "StartConversation" or "PlayFirstTimeCutscene"
        cutsceneManager.Invoke("StartConversation", 0f);
      }
    }
    else
    {
      Debug.LogWarning("Cutscene manager not assigned on HermitInteractionController.");
      // immediately end if no manager assigned (avoid locking player)
      EndInteraction(false);
    }
  }

  public void EndInteraction(bool disableBlocker = true, bool resumeHumming = true)
  {
    conversationActive = false;
    initialCutsceneDone = true;

    // enable mixer option next time
    //if (optionPanel != null && playerInRange)
    //{
    //  optionPanel.SetActive(true);
    //  if (mixerOption != null) mixerOption.SetActive(true);
    //}

    // disable blocker so player can pass
    if (disableBlocker && blockerObject != null)
      blockerObject.SetActive(false);

    // re-enable player
    if (playerBridge != null) playerBridge.EnableMovement();
    else
    {
      var cc = player?.GetComponent<CharacterController>();
      if (cc != null) cc.enabled = true;
      var rb = player?.GetComponent<Rigidbody>();
      if (rb != null) rb.isKinematic = false;
    }

    if (resumeHumming) hermitAudio?.FadeInHumming();

    OnEndConversation?.Invoke();
  }

  private void OpenMixer()
  {
    if (mixerPanel != null && !mixerPanel.activeSelf)
    {
      mixerPanel.SetActive(true);
      Debug.Log("Mixer opened");
    }
  }

  // This method can be used by UI buttons for Interact/Mixer if needed
  public void OnInteractButtonPressed()
  {
    if (conversationActive) return;
    StartInteraction();
  }

  public void OnMixerButtonPressed()
  {
    if (conversationActive) return;
    optionPanel.SetActive(false);
    hermitAudio?.FadeOutHumming();
    OpenMixer();
  }
}
