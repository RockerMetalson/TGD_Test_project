using UnityEngine;
using System.Collections.Generic;

public class SeedSelectorUI : MonoBehaviour
{
    public static SeedData ActiveSeed { get; private set; }
    public List<SeedData> availableSeeds = new List<SeedData>();

    private int currentIndex = 0;

    private void Start()
    {
        if (availableSeeds.Count > 0)
            ActiveSeed = availableSeeds[0];
    }

    private void Update()
    {
        // Switch seeds with number keys 1-4
        for (int i = 0; i < availableSeeds.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ActiveSeed = availableSeeds[i];
                Debug.Log($"Selected seed: {ActiveSeed.seedName}");
            }
        }
    }
}
