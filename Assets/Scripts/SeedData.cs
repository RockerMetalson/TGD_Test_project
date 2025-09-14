using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Farming/Seed")]
public class SeedData : ScriptableObject
{
    public string seedName;

    // Initial planted prefab (e.g., tiny seed or sprout)
    public GameObject seedPrefab;

    // Growth stages (size = 4)
    public GameObject[] growthPrefabs = new GameObject[4];

    public float totalGrowTime = 30f; // Total time for full growth
    public int seedCost = 5;          // Buying price
    public int sellValue = 10;        // Value when harvested
}
