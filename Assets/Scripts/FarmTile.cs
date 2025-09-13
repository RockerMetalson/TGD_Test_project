using UnityEngine;

public enum TileState { Green, Plowed } // Only define this ONCE!

public class FarmTile : MonoBehaviour, IInteractable
{
    [Header("Tile State")]
    public TileState currentState = TileState.Green;

    [Header("Visual References")]
    public GameObject greenVisual;    // e.g., 3D_Tile_Ground_01
    public GameObject plowedVisual;   // e.g., 3D_Tile_Farm_Field_01

    private void Awake()
    {
        ApplyVisuals();
    }

    public string GetPromptMessage()
    {
        return currentState switch
        {
            TileState.Green => "Press E to Plow",
            TileState.Plowed => "Press E to Plant Seeds",
            _ => ""
        };
    }

    public void Interact()
    {
        Debug.Log($"[FarmTile] Interact() called on {name}, current state = {currentState}");

        if (currentState == TileState.Green)
        {
            currentState = TileState.Plowed;
            Debug.Log($"[FarmTile] {name} is now Plowed!");
        }

        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (!greenVisual || !plowedVisual)
        {
            Debug.LogError($"[FarmTile] Missing visual references on {name}!");
            return;
        }

        greenVisual.SetActive(currentState == TileState.Green);
        plowedVisual.SetActive(currentState == TileState.Plowed);

        Debug.Log($"[FarmTile] Visuals updated on {name}: Green={greenVisual.activeSelf}, Plowed={plowedVisual.activeSelf}");
    }
}
