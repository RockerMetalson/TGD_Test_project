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
        // Auto-reset plowed tiles back to Green if no seed was planted
        if (currentState == TileState.Plowed)
        {
            plowedTimer += Time.deltaTime;
            if (plowedTimer >= autoResetTime)
                SetState(TileState.Green);
        }
    }

    public void SetState(TileState newState)
    {
        currentState = newState;
        plowedTimer = 0f;

        greenVisual.SetActive(currentState == TileState.Green);
        plowedVisual.SetActive(currentState == TileState.Plowed || currentState == TileState.Planted);
    }

    public void Interact()
    {
        switch (currentState)
        {
            case TileState.Green:
                SetState(TileState.Plowed);
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

    private void TryPlantSeed()
    {
        if (SeedSelectorUI.ActiveSeed == null)
        {
            Debug.LogWarning("No seed selected to plant.");
            return;
        }

        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (!inventory.UseSeed(SeedSelectorUI.ActiveSeed))
        {
            Debug.LogWarning($"No seeds of type {SeedSelectorUI.ActiveSeed.seedName} in inventory.");
            return;
        }

        GameObject cropHolder = new GameObject($"Crop_{SeedSelectorUI.ActiveSeed.seedName}");
        cropHolder.transform.SetParent(cropSpawnPoint);
        cropHolder.transform.localPosition = Vector3.zero;
        cropHolder.transform.localRotation = Quaternion.identity;

        Crop crop = cropHolder.AddComponent<Crop>();
        crop.Initialize(SeedSelectorUI.ActiveSeed);
        activeCrop = crop;

        SetState(TileState.Planted);
    }

    private void HarvestCrop()
    {
        if (activeCrop != null)
        {
            activeCrop.Harvest();
            activeCrop = null;
        }

        SetState(TileState.Green);
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
                if (activeCrop != null && activeCrop.IsReadyToHarvest)
                    return "Press E to Harvest";
                return "Growing...";
            default:
                return "";
        }
    }
}
