using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI selectedSeedText;

    void Update()
    {
        UpdateMoneyUI();
        UpdateSelectedSeedUI();
    }

    private void UpdateMoneyUI()
    {
        if (playerInventory != null && moneyText != null)
        {
            moneyText.text = $"$ {playerInventory.money}";
        }
    }

    private void UpdateSelectedSeedUI()
    {
        if (SeedSelectorUI.ActiveSeed != null && selectedSeedText != null)
        {
            // Find seed in inventory to show count
            var slot = playerInventory.seedInventory.Find(s => s.seedData == SeedSelectorUI.ActiveSeed);
            int quantity = slot != null ? slot.quantity : 0;

            selectedSeedText.text = $"Seed: {SeedSelectorUI.ActiveSeed.seedName} (x{quantity})";
        }
        else
        {
            selectedSeedText.text = "No seed selected";
        }
    }
}
