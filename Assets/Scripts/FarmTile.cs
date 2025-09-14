using UnityEngine;

public enum TileState { Green, Plowed, Planted }

public class FarmTile : MonoBehaviour, IInteractable
{
    [Header("Tile Settings")]
    public TileState currentState = TileState.Green;
    public float plowedResetTime = 15f;
    private float plowedTimer;

    [Header("Visuals")]
    public GameObject greenVisual;
    public GameObject plowedVisual;

    [Header("Crop Spawn Point")]
    public Transform cropSpawnPoint;

    private Crop activeCrop;

    public string GetPromptMessage()
    {
        return currentState switch
        {
            TileState.Green => "Press E to Plow",
            TileState.Plowed => "Press E to Plant",
            TileState.Planted when activeCrop != null && activeCrop.IsReadyToHarvest => "Press E to Harvest",
            _ => ""
        };
    }

    public void Interact()
    {
        switch (currentState)
        {
            case TileState.Green:
                PlowTile();
                break;

            case TileState.Plowed:
                TryPlantSeed();
                break;

            case TileState.Planted:
                if (activeCrop != null && activeCrop.IsReadyToHarvest)
                {
                    HarvestCrop();
                }
                break;
        }
    }

    private void Update()
    {
        if (currentState == TileState.Plowed && activeCrop == null)
        {
            plowedTimer += Time.deltaTime;
            if (plowedTimer >= plowedResetTime)
            {
                currentState = TileState.Green;
                ApplyVisuals();
                Debug.Log("Tile reverted to Green (time expired).");
            }
        }
        else
        {
            plowedTimer = 0f;
        }
    }

    private void PlowTile()
    {
        currentState = TileState.Plowed;
        ApplyVisuals();
        Debug.Log("Tile plowed.");
    }

    private void TryPlantSeed()
    {
        var selectedSeed = SeedSelectorUI.ActiveSeed;

        // 1. Ensure a seed is selected
        if (selectedSeed == null)
        {
            Debug.LogWarning("No seed type selected in Seed Selector!");
            return;
        }

        // 2. Check if player has this seed in inventory
        var inventory = FindObjectOfType<PlayerInventory>();
        if (!inventory.UseSeed(selectedSeed))
        {
            Debug.LogWarning($"Cannot plant {selectedSeed.seedName} — none in inventory!");
            return;
        }

        // 3. Spawn crop object
        GameObject cropObject = new GameObject("Crop_" + selectedSeed.seedName);
        cropObject.transform.position = cropSpawnPoint.position;
        cropObject.transform.SetParent(cropSpawnPoint);

        Crop cropComponent = cropObject.AddComponent<Crop>();
        cropComponent.Initialize(selectedSeed);

        activeCrop = cropComponent;

        // 4. Update tile state
        currentState = TileState.Planted;
        plowedTimer = 0f;

        Debug.Log($"{selectedSeed.seedName} planted successfully. Remaining: {inventory.seedInventory.Find(s => s.seedData == selectedSeed)?.quantity}");
    }




    private void HarvestCrop()
    {
        activeCrop.Harvest();
        activeCrop = null;
        currentState = TileState.Green;
        ApplyVisuals();
        Debug.Log("Crop harvested, tile reset to Green.");
    }

    private void ApplyVisuals()
    {
        greenVisual.SetActive(currentState == TileState.Green);
        plowedVisual.SetActive(currentState == TileState.Plowed);
    }
}
