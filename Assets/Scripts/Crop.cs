using UnityEngine;

public class Crop : MonoBehaviour
{
    public SeedData seedData;            // Set in Initialize
    public float growthTimer = 0f;

    private int currentStage = -1;       // -1 so first spawn always happens
    private GameObject currentVisual;    // The spawned instance for the current stage

    public bool IsReadyToHarvest => seedData != null && growthTimer >= seedData.totalGrowTime;

    public void Initialize(SeedData data)
    {
        seedData = data;
        growthTimer = 0f;
        currentStage = -1;
        SpawnStage(0);
    }

    private void Update()
    {
        if (seedData == null || seedData.growthPrefabs == null || seedData.growthPrefabs.Length == 0) return;

        growthTimer += Time.deltaTime;

        float stageDuration = seedData.totalGrowTime / seedData.growthPrefabs.Length;
        int targetStage = Mathf.FloorToInt(growthTimer / stageDuration);
        targetStage = Mathf.Clamp(targetStage, 0, seedData.growthPrefabs.Length - 1);

        if (targetStage != currentStage)
        {
            SpawnStage(targetStage);

            // Optional: as soon as final stage appears, mark ready immediately
            if (targetStage == seedData.growthPrefabs.Length - 1)
            {
                growthTimer = seedData.totalGrowTime;
                Debug.Log($"{seedData.seedName} reached final stage → ready to harvest.");
            }
        }
    }

    private void SpawnStage(int stage)
    {
        if (seedData.growthPrefabs[stage] == null)
        {
            Debug.LogWarning($"Growth prefab missing for stage {stage} on {seedData.seedName}");
            return;
        }

        if (currentVisual != null) Destroy(currentVisual);

        currentVisual = Instantiate(
            seedData.growthPrefabs[stage],
            transform.position,
            Quaternion.identity,
            transform
        );

        currentStage = stage;
        Debug.Log($"{seedData.seedName} → stage {stage + 1}");
    }

    public void Harvest()
    {
        if (currentVisual != null) Destroy(currentVisual);
        Destroy(gameObject);
    }
}
