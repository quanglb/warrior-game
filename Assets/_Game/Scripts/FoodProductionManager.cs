using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Game.Scripts
{
    public class FoodProductionManager : Singleton<FoodProductionManager>
    {
        public Image progressBar; // Image component for the progress bar
        public TextMeshProUGUI foodCountText; // TextMeshProUGUI to display the food count
        private float productionSpeed; // Speed of food production (based on MeatRegenSpeed)
        private int currentFood; // Current food count

        public int CurrentFood
        {
            get {return currentFood;}
            set
            {
                currentFood = value;
                UpdateFoodUI();
            }
        }

        private bool isProducing; // Whether food production is active


        private void Start()
        {
            // Initialize values
            currentFood = 0;
            progressBar.fillAmount = 0f;
            productionSpeed = DataManager.MeatRegenSpeed;
            UpdateFoodUI();
        }

        private void Update()
        {
            // Only produce food if the level is started
            if (isProducing)
            {
                ProduceFood();
            }
        }

        private void ProduceFood()
        {
            // Increment the fill amount based on the production speed
            progressBar.fillAmount += productionSpeed * Time.deltaTime;

            // Check if the bar is full
            if (progressBar.fillAmount >= 1f)
            {
                // Reset the bar and increment food count
                progressBar.fillAmount = 0f;
                currentFood += 1;
                UpdateFoodUI();
            }
        }

        private void UpdateFoodUI()
        {
            foodCountText.text = $"{currentFood}F";
            
            LevelManager.Instance.UpdateButtonStatus(currentFood);
        }

        public void StartProduction()
        {
            isProducing = true;
        }

        public void StopProduction()
        {
            isProducing = false;
        }
    }
}
