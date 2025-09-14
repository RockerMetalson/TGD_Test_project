using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI interactionPromptText;

    private FarmTile currentTile;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FarmTile>(out FarmTile tile))
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
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentTile != null)
        {
            UpdatePrompt();

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentTile.Interact();
                UpdatePrompt();
            }
        }
    }

    private void UpdatePrompt()
    {
        if (!currentTile) return;
        string text = currentTile.GetInteractionText();
        interactionPromptText.text = text;
        interactionPromptText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}
