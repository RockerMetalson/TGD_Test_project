using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Farming/Seed")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public float totalGrowTime = 30f;

    // EXACTLY these 4 (or more) prefabs for the growth stages, in order
    public GameObject[] growthPrefabs = new GameObject[4];
}
