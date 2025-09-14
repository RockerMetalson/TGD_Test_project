using UnityEngine;

public class Crop : MonoBehaviour
{
    public SeedData seedData;
    private float growthTimer;
    private int currentStage = -1;
    private GameObject currentVisual;

    public bool IsReadyToHarvest => growthTimer >= seedData.totalGrowTime;

    public void Initialize(SeedData seed)
    {
        seedData = seed;
        growthTimer = 0f;
        UpdateStageVisual(0); // start with stage 1
    }

    private void Update()
    {
        growthTimer += Time.deltaTime;

        float stageDuration = seedData.totalGrowTime / seedData.growthPrefabs.Length;
        int stage = Mathf.FloorToInt(growthTimer / stageDuration);
        stage = Mathf.Min(stage, seedData.growthPrefabs.Length - 1);

        if (stage != currentStage)
        {
            UpdateStageVisual(stage);

            // If we just reached the LAST stage → instantly ready to harvest
            if (stage == seedData.growthPrefabs.Length - 1)
            {
                growthTimer = seedData.totalGrowTime; // instantly mark as fully grown
                Debug.Log($"{seedData.seedName} is now fully grown and ready to harvest!");
            }
        }
    }


    private void UpdateStageVisual(int stage)
    {
        if (currentVisual != null)
            Destroy(currentVisual);

        currentVisual = Instantiate(seedData.growthPrefabs[stage], transform.position, Quaternion.identity, transform);
        currentStage = stage;
        Debug.Log($"{seedData.seedName} grew to stage {stage + 1}");
    }

    public void Harvest()
    {
        if (currentVisual != null)
            Destroy(currentVisual);

        Destroy(gameObject); // remove entire crop object
    }
}
