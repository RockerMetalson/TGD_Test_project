using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f; // How far the player can interact

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;  // Drag TMP text here in Inspector

    private Camera mainCamera;
    private IInteractable currentTarget;

    private void Start()
    {
        mainCamera = Camera.main;
        if (!mainCamera)
            Debug.LogError("[PlayerInteractor] Main Camera not found! Tag your camera as MainCamera.");

        if (!promptText)
            Debug.LogError("[PlayerInteractor] Prompt Text not assigned!");

        if (promptText)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleLookAtTarget();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[PlayerInteractor] Interacting with current target.");
            currentTarget.Interact();
        }
    }

    private void HandleLookAtTarget()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        // Use ONLY the FarmTile layer, while automatically ignoring the Player
        int farmTileMask = LayerMask.GetMask("FarmTile"); // Ray will only hit this layer

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, farmTileMask))
        {
            Debug.Log($"[PlayerInteractor] Ray hit: {hit.collider.name}");

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
                else
                {
                    Debug.LogWarning("[PlayerInteractor] Tagged object but no IInteractable found!");
                }
            }
            else
            {
                Debug.LogWarning("[PlayerInteractor] Hit object on FarmTile layer but without Interactable tag.");
            }
        }
        else
        {
            Debug.Log("[PlayerInteractor] Ray did not hit anything.");
        }

        // Hide prompt when nothing interactable is hit
        currentTarget = null;
        promptText.gameObject.SetActive(false);
    }
}
