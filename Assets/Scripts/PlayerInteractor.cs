using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;

    [Header("Player Components")]
    public Animator playerAnimator;          
    public AudioSource audioSource;          
    public AudioClip plowSound;              

    private Camera mainCamera;
    private IInteractable currentTarget;

    private void Start()
    {
        mainCamera = Camera.main;
        if (!mainCamera) Debug.LogError("[PlayerInteractor] Main Camera not found!");

        if (!promptText) Debug.LogError("[PlayerInteractor] Prompt Text not assigned!");
        else promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleLookAtTarget();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[PlayerInteractor] Interacting with current target.");

            // Play animation always
            if (playerAnimator)
                playerAnimator.SetTrigger("Pickup");

            // Play sound ONLY if the tile is currently Green
            if (currentTarget is FarmTile farmTile && farmTile.currentState == TileState.Green)
            {
                if (audioSource && plowSound)
                {
                    audioSource.PlayOneShot(plowSound);
                    Debug.Log("[PlayerInteractor] Plow sound played for Green tile.");
                }
            }

            currentTarget.Interact();
        }
    }

    private void PlayPickupAnimationAndSound()
    {
        // Play Pickup animation
        if (playerAnimator)
        {
            playerAnimator.SetTrigger("Pickup");
            Debug.Log("[PlayerInteractor] Pickup animation triggered.");
        }

        // Play sound effect
        if (audioSource && plowSound)
        {
            audioSource.PlayOneShot(plowSound);
            Debug.Log("[PlayerInteractor] Plow sound played.");
        }
    }

    private void HandleLookAtTarget()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        int farmTileMask = LayerMask.GetMask("FarmTile");

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, farmTileMask))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    currentTarget = interactable;
                    promptText.text = currentTarget.GetPromptMessage();
                    promptText.gameObject.SetActive(true);
                    return;
                }
            }
        }

        currentTarget = null;
        promptText.gameObject.SetActive(false);
    }
}
