using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string pickupTriggerName = "Pickup"; // Supercyan controller uses this

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip plowSound;
    [SerializeField] private AudioClip plantSound;
    [SerializeField] private AudioClip harvestSound;

    private FarmTile currentTile;

    private void Awake()
    {
        // Auto-wire if not set in Inspector
        if (!playerAnimator) playerAnimator = GetComponentInChildren<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FarmTile>(out var tile))
        {
            currentTile = tile;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentTile != null && other.gameObject == currentTile.gameObject)
        {
            currentTile = null;
            if (interactionPromptText) interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentTile == null) return;

        UpdatePrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayPickupAnimation();          // ← plays the Supercyan "Pickup" anim
            PlayContextSound(currentTile);  // ← different sound per action
            currentTile.Interact();         // ← tile logic (plow/plant/harvest)
            UpdatePrompt();
        }
    }

    private void UpdatePrompt()
    {
        if (!interactionPromptText || currentTile == null) return;

        string text = currentTile.GetInteractionText();
        interactionPromptText.text = text;
        interactionPromptText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    private void PlayPickupAnimation()
    {
        if (!playerAnimator) return;
        // in case other triggers linger
        playerAnimator.ResetTrigger(pickupTriggerName);
        playerAnimator.SetTrigger(pickupTriggerName);
    }

    private void PlayContextSound(FarmTile tile)
    {
        if (!audioSource) return;

        switch (tile.currentState)
        {
            case TileState.Green:   // we are about to plow
                if (plowSound) audioSource.PlayOneShot(plowSound);
                break;

            case TileState.Plowed:  // we are about to plant
                if (plantSound) audioSource.PlayOneShot(plantSound);
                break;

            case TileState.Planted: // we might harvest (only if ready)
                if (tile.activeCrop != null && tile.activeCrop.IsReadyToHarvest)
                    if (harvestSound) audioSource.PlayOneShot(harvestSound);
                break;
        }
    }
}
