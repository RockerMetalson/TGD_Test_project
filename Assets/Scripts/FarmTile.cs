using UnityEngine;

public enum TileState { Green, Plowed, Planted }

public class FarmTile : MonoBehaviour
{
    [Header("Tile Visuals")]
    public GameObject greenVisual;
    public GameObject plowedVisual;
    public Transform cropSpawnPoint;

    [Header("Timing")]
    public float autoResetTime = 10f;

    [Header("State")]
    public TileState currentState = TileState.Green;
    public Crop activeCrop;

    private float plowedTimer = 0f;

    private void Update()
    {
        // Auto-reset plowed → green if nothing planted
        if (currentState == TileState.Plowed)
        {
            plowedTimer += Time.deltaTime;
            if (plowedTimer >= autoResetTime) SetState(TileState.Green);
        }
    }

    public void SetState(TileState newState)
    {
        currentState = newState;
        plowedTimer = 0f;

        // Green on Green only
        greenVisual.SetActive(currentState == TileState.Green);
        // Keep plowed mesh visible for both Plowed and Planted
        plowedVisual.SetActive(currentState == TileState.Plowed || currentState == TileState.Planted);
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
                    HarvestCrop();
                break;
        }
    }

    private void PlowTile()
    {
        Debug.Log("Tile plowed");
        SetState(TileState.Plowed);
    }

    private void TryPlantSeed()
    {
        if (SeedSelectorUI.ActiveSeed == null)
        {
            Debug.LogWarning("No seed selected.");
            return;
        }

        var inventory = FindObjectOfType<PlayerInventory>();
        if (!inventory.UseSeed(SeedSelectorUI.ActiveSeed))
        {
            Debug.LogWarning($"No '{SeedSelectorUI.ActiveSeed.seedName}' seeds in inventory.");
            return;
        }

        // Create a crop holder object under the spawn point
        var cropGO = new GameObject($"Crop_{SeedSelectorUI.ActiveSeed.seedName}");
        cropGO.transform.SetParent(cropSpawnPoint);
        cropGO.transform.localPosition = Vector3.zero;
        cropGO.transform.localRotation = Quaternion.identity;

        var crop = cropGO.AddComponent<Crop>();
        crop.Initialize(SeedSelectorUI.ActiveSeed);
        activeCrop = crop;

        SetState(TileState.Planted);
        Debug.Log($"{SeedSelectorUI.ActiveSeed.seedName} planted.");
    }

    private void HarvestCrop()
    {
        if (activeCrop != null)
        {
            activeCrop.Harvest();
            activeCrop = null;
        }
        SetState(TileState.Green);
        Debug.Log("Crop harvested → tile back to Green.");
    }

    public string GetInteractionText()
    {
        switch (currentState)
        {
            case TileState.Green:
                return "Press E to Plow";
            case TileState.Plowed:
                return "Press E to Plant";
            case TileState.Planted:
                if (activeCrop != null && activeCrop.IsReadyToHarvest) return "Press E to Harvest";
                return "Growing...";
            default:
                return "";
        }
    }
}
