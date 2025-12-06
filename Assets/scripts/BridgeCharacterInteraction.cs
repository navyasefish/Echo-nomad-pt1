using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BridgeCharacterInteraction : MonoBehaviour
{
  [Header("UI References")]
  public GameObject interactPrompt; // "[E] Interact" UI
  public GameObject dialoguePanel; // Main dialogue container
  public GameObject[] dialoguePanels; // Array of individual dialogue panels
  public GameObject mixerPanel; // Mixer UI panel
  public GameObject mixerButton; // The [M] Mixer button (child of interactPrompt)

  [Header("Bridge Collider")]
  public GameObject bridgeBlocker; // The collider blocking the bridge

  [Header("Audio")]
  public AudioSource bgMusicSource; // Background music during cutscene
  public AudioSource sfxSource; // For button click sounds
  public AudioClip clickSound; // Space bar click sound
  public AudioClip[] panelSounds; // Character sounds for each panel (huh, hmm, etc.)

  [Header("Settings")]
  public float typingDelay = 0.05f; // For text typing effect if needed

  private bool playerInRange = false;
  private bool firstInteractionComplete = false;
  private bool inCutscene = false;
  private int currentPanelIndex = 0;

  void Start()
  {
    // Hide all UI elements at start
    interactPrompt.SetActive(false);
    dialoguePanel.SetActive(false);
    mixerPanel.SetActive(false);

    // Hide mixer button initially (shown after first interaction)
    if (mixerButton != null) mixerButton.SetActive(false);

    // Hide all dialogue panels
    foreach (GameObject panel in dialoguePanels)
    {
      panel.SetActive(false);
    }
  }

  void Update()
  {
    // Only process input when player is in range and not in cutscene
    if (playerInRange && !inCutscene)
    {
      // E key for interaction
      if (Input.GetKeyDown(KeyCode.E))
      {
        if (!firstInteractionComplete)
        {
          StartFirstInteraction();
        }
        else
        {
          // Show repeat dialogue
          OnInteractButtonPressed();
        }
      }

      // M key for mixer (only after first interaction)
      if (firstInteractionComplete && Input.GetKeyDown(KeyCode.M))
      {
        OnMixerButtonPressed();
      }
    }

    // Handle dialogue progression with Space
    if (inCutscene && Input.GetKeyDown(KeyCode.Space))
    {
      PlayClickSound();
      NextDialoguePanel();
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = true;

      // Show interact prompt panel
      interactPrompt.SetActive(true);

      // Show mixer button only after first interaction
      if (firstInteractionComplete && mixerButton != null)
      {
        mixerButton.SetActive(true);
      }
    }
  }

  void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = false;
      interactPrompt.SetActive(false);
    }
  }

  void StartFirstInteraction()
  {
    inCutscene = true;
    currentPanelIndex = 0;

    // Hide prompt
    interactPrompt.SetActive(false);

    // Disable player movement (you'll need to reference your player controller)
    DisablePlayerMovement();

    // Start background music
    if (bgMusicSource != null && bgMusicSource.clip != null)
    {
      bgMusicSource.Play();
    }

    // Show dialogue panel
    dialoguePanel.SetActive(true);

    // Show first panel
    ShowDialoguePanel(0);
  }

  void ShowDialoguePanel(int index)
  {
    // Hide all panels first
    foreach (GameObject panel in dialoguePanels)
    {
      panel.SetActive(false);
    }

    // Show current panel
    if (index < dialoguePanels.Length)
    {
      dialoguePanels[index].SetActive(true);

      // Play panel-specific sound
      if (index < panelSounds.Length && panelSounds[index] != null)
      {
        sfxSource.PlayOneShot(panelSounds[index]);
      }
    }
  }

  void NextDialoguePanel()
  {
    currentPanelIndex++;

    if (currentPanelIndex < dialoguePanels.Length)
    {
      // Show next panel
      ShowDialoguePanel(currentPanelIndex);
    }
    else
    {
      // End of dialogue
      EndFirstInteraction();
    }
  }

  void EndFirstInteraction()
  {
    inCutscene = false;
    firstInteractionComplete = true;

    // Hide dialogue panel
    dialoguePanel.SetActive(false);

    // Stop background music
    if (bgMusicSource != null)
    {
      bgMusicSource.Stop();
    }

    // Re-enable player movement
    EnablePlayerMovement();

    // If player still in range, show mixer button
    if (playerInRange)
    {
      interactPrompt.SetActive(true);
      if (mixerButton != null) mixerButton.SetActive(true);
    }
  }

  // Called by button or E key after first interaction
  public void OnInteractButtonPressed()
  {
    // Hide interact prompt
    interactPrompt.SetActive(false);

    dialoguePanel.SetActive(true);

    // Show only the first panel (or create a specific "quest reminder" panel)
    ShowDialoguePanel(0); // You might want to use a different panel index for this

    // Play sound
    if (panelSounds.Length > 0 && panelSounds[0] != null)
    {
      sfxSource.PlayOneShot(panelSounds[0]);
    }

    StartCoroutine(CloseRepeatDialogue());
  }

  IEnumerator CloseRepeatDialogue()
  {
    // Wait for player to press Space to close
    while (!Input.GetKeyDown(KeyCode.Space))
    {
      yield return null;
    }

    PlayClickSound();
    dialoguePanel.SetActive(false);

    if (playerInRange)
    {
      interactPrompt.SetActive(true);
      if (mixerButton != null) mixerButton.SetActive(true);
    }
  }

  // Called by button or M key
  public void OnMixerButtonPressed()
  {
    interactPrompt.SetActive(false);
    mixerPanel.SetActive(true);

    // Play click sound
    PlayClickSound();
  }

  // Call this when player completes the quest
  public void CompleteQuest()
  {
    if (bridgeBlocker != null)
    {
      bridgeBlocker.SetActive(false);
    }
  }

  public void CloseMixer()
  {
    mixerPanel.SetActive(false);

    if (playerInRange)
    {
      interactPrompt.SetActive(true);
      if (mixerButton != null) mixerButton.SetActive(true);
    }
  }

  void PlayClickSound()
  {
    if (sfxSource != null && clickSound != null)
    {
      sfxSource.PlayOneShot(clickSound);
    }
  }

  void DisablePlayerMovement()
  {
    // You'll need to get reference to your player controller
    // Example:
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
      // Disable the third person controller component
      var controller = player.GetComponent<CharacterController>();
      if (controller != null) controller.enabled = false;

      // Or disable your specific movement script
      // var movement = player.GetComponent<YourMovementScript>();
      // if (movement != null) movement.enabled = false;
    }
  }

  void EnablePlayerMovement()
  {
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
      var controller = player.GetComponent<CharacterController>();
      if (controller != null) controller.enabled = true;

      // Or re-enable your specific movement script
      // var movement = player.GetComponent<YourMovementScript>();
      // if (movement != null) movement.enabled = true;
    }
  }
}