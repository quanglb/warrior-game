using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts;

public class HomeUI : MonoBehaviour
{
    public Button playButton;
    public Button upgradeFood;
    public Button upgradeHealthBase;
    public TextMeshProUGUI upgradeFoodPrice;
    public TextMeshProUGUI upgradeHealthBasePrice;
    public TextMeshProUGUI upgradeFoodValue;
    public TextMeshProUGUI upgradeHealthBaseValue;

    public GameObject panelUI;

    private void Start()
    {
        // Initialize buttons
        upgradeFood.onClick.AddListener(
            () => UpgradeFeature(DataKey.MeatRegenSpeed, upgradeFoodPrice, upgradeFoodValue));
        upgradeHealthBase.onClick.AddListener(() =>
            UpgradeFeature(DataKey.TowerLevel, upgradeHealthBasePrice, upgradeHealthBaseValue));

        // Update prices and values on UI
        UpdateUpgradeUI();

        playButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.Play();
            panelUI.SetActive(false);
        });
    }

    private void UpgradeFeature(DataKey featureKey, TextMeshProUGUI priceText, TextMeshProUGUI valueText)
    {
        // Fetch the current level based on the feature key
        int currentLevel = featureKey == DataKey.MeatRegenSpeed
            ? Mathf.FloorToInt(DataManager.MeatRegenSpeed / 0.3f)
            : DataManager.TowerLevel;
    
        // Calculate the upgrade cost
        int upgradeCost = 2 + 2 * currentLevel;

        // Check if the player has enough coins
        if (DataManager.currCoin >= upgradeCost)
        {
            // Deduct coins and increase level
            DataManager.SetCoin(DataManager.currCoin - upgradeCost);

            // Update the feature value
            if (featureKey == DataKey.MeatRegenSpeed)
            {
                DataManager.MeatRegenSpeed += 0.3f;
            }
            else if (featureKey == DataKey.TowerLevel)
            {
                DataManager.TowerLevel += 1;
            }

            // Log the upgrade success
            Debug.Log($"Upgraded {featureKey} to level {currentLevel + 1} for {upgradeCost} coins.");

            // Update the price text
            priceText.text = (2 + 2 * (currentLevel + 1)).ToString() + "G";

            // Update the value text
            if (featureKey == DataKey.MeatRegenSpeed)
            {
                valueText.text = $"Food Production: {(0.2f + (currentLevel + 1) * 0.3f):F2}";
            }
            else if (featureKey == DataKey.TowerLevel)
            {
                valueText.text = $"Base Health: {(2 + (currentLevel + 1) * 4)}";
            }
        }
        else
        {
            // Log a warning if the player lacks sufficient coins
            Debug.LogWarning("Not enough coins to upgrade.");
        }
    }


    private void UpdateUpgradeUI()
    {
        int foodLevel = Mathf.FloorToInt(DataManager.MeatRegenSpeed / 0.3f);
        int healthBaseLevel = DataManager.TowerLevel;

        upgradeFoodPrice.text = (2 * foodLevel).ToString() + "G";
        upgradeHealthBasePrice.text = (2 * healthBaseLevel).ToString() + "G";

        upgradeFoodValue.text = $"Food Production: {(0.2f + foodLevel * 0.3f).ToString("F2")}";
        upgradeHealthBaseValue.text = $"Base Health: {(2 + healthBaseLevel * 4).ToString()}";
    }
}