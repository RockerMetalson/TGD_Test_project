using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private float warningPromptDuration = 1.5f; // how long to show warning text

    private Coroutine warningCoroutine; // track running warning
    private bool showingWarning = false; // prevent prompt from updating while warning is active

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string pickupStateName = "Pickup"; // Animator state name for Pickup
    [SerializeField] private int pickupLayerIndex = 1; // 0 = Base Layer, 1 = Pickup Layer

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip plowSound;
    [SerializeField] private AudioClip plantSound;
    [SerializeField] private AudioClip harvestSound;

    private FarmTile currentTile;

    private static readonly int InteractTrigger = Animator.StringToHash("Pickup");

    private void Awake()
    {
        if (!playerAnimator) playerAnimator = GetComponentInChildren<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentTile != null)
        {
            // Only update prompt if a warning isn't currently showing
            if (!showingWarning)
                UpdatePrompt();

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Check if action is valid and animation isn't playing
                if (CanTriggerAnimation(currentTile) && !IsPickupAnimationPlaying())
                {
                    // === Check planting action for available seeds ===
                    if (currentTile.currentState == TileState.Plowed)
                    {
                        var inventory = FindObjectOfType<PlayerInventory>();
                        var slot = inventory.seedInventory.Find(s => s.seedData == SeedSelectorUI.ActiveSeed);

                        if (slot == null || slot.quantity <= 0)
                        {
                            // Show warning if no seeds available
                            ShowTemporaryWarning("You don't have any seeds of this type!");
                            return; // stop here, no animation or planting
                        }
                    }

                    // Trigger animation and sound
                    playerAnimator.ResetTrigger(InteractTrigger);
                    playerAnimator.SetTrigger(InteractTrigger);
                    PlayContextSound(currentTile);

                    // Execute interaction
                    currentTile.Interact();
                    UpdatePrompt();
                }
                else if (IsPickupAnimationPlaying())
                {
                    Debug.Log("Pickup animation still playing, skipping trigger.");
                }
            }
        }
    }

    /// <summary>
    /// Displays a temporary warning message on the prompt text.
    /// </summary>
    private void ShowTemporaryWarning(string message)
    {
        if (interactionPromptText == null) return;

        // Stop any running warning before starting a new one
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(WarningPromptRoutine(message));
    }

    private IEnumerator WarningPromptRoutine(string message)
    {
        showingWarning = true;

        // Show warning message
        interactionPromptText.text = message;

        // Wait for duration
        yield return new WaitForSeconds(warningPromptDuration);

        showingWarning = false;
        UpdatePrompt(); // revert to normal prompt
    }

    /// <summary>
    /// Checks if the Pickup animation is currently active and hasn't finished yet.
    /// </summary>
    private bool IsPickupAnimationPlaying()
    {
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(pickupLayerIndex);
        return stateInfo.IsName(pickupStateName) && stateInfo.normalizedTime < 1.0f;
    }

    private void UpdatePrompt()
    {
        if (!interactionPromptText || currentTile == null) return;

        string text = currentTile.GetInteractionText();
        interactionPromptText.text = text;
        interactionPromptText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    /// <summary>
    /// Determines if an animation should be triggered based on the current tile state.
    /// </summary>
    private bool CanTriggerAnimation(FarmTile tile)
    {
        switch (tile.currentState)
        {
            case TileState.Green:
                return true; // Plowing
            case TileState.Plowed:
                return true; // Planting
            case TileState.Planted:
                return tile.activeCrop != null && tile.activeCrop.IsReadyToHarvest; // Harvest
            default:
                return false;
        }
    }

    private void PlayContextSound(FarmTile tile)
    {
        if (!audioSource) return;

        switch (tile.currentState)
        {
            case TileState.Green:
                if (plowSound) audioSource.PlayOneShot(plowSound);
                break;

            case TileState.Plowed:
                if (plantSound) audioSource.PlayOneShot(plantSound);
                break;

            case TileState.Planted:
                if (tile.activeCrop != null && tile.activeCrop.IsReadyToHarvest)
                    if (harvestSound) audioSource.PlayOneShot(harvestSound);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var tile = other.GetComponentInParent<FarmTile>();
        if (tile == null) return;

        currentTile = tile;
        UpdatePrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        var tile = other.GetComponentInParent<FarmTile>();
        if (tile != null && tile == currentTile)
        {
            currentTile = null;
            if (interactionPromptText)
                interactionPromptText.gameObject.SetActive(false);
        }
    }
}
