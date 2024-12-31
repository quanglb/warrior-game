using UnityEngine;

namespace _Game.Scripts
{
    public class Tower : MonoBehaviour
    {
        public Team team;
        public int initialHealth;

        [SerializeField] private HealthSystemForDummies healthBar;

        private void Start()
        {
            if (Team.Player == team)
            {
                initialHealth = DataManager.TowerLevel * 2;
            }

            healthBar.MaximumHealth = initialHealth;
            healthBar.CurrentHealth = initialHealth;

        }
    }
}
